#region

using LoESoft.AppEngine.sfx;
using LoESoft.Core.config;
using LoESoft.Core.models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

#endregion

namespace LoESoft.AppEngine
{
    public class AppEngineManager
    {
        public int PORT
        { get { return Settings.APPENGINE.PRODUCTION_PORT; } }

        public HttpListener _websocket
        { get; private set; }

        public Queue<HttpListenerContext> _webqueue
        { get; private set; }

        public ManualResetEvent _webevent
        { get; private set; }

        private Thread[] _webthread
        { get; set; }

        private object _weblock
        { get; set; }

        public bool _shutdown
        { get; set; }

        public bool _isCompleted
        { get; private set; }

        protected bool _restart
        { get; set; }

        public delegate bool WebSocketDelegate();

        public AppEngineManager(
            bool restart
            )
        {
            _restart = restart;
        }

        public void Start()
        {
            Thread.CurrentThread.Name = "Entry";
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            if (!IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpConnections().All(_ => _.LocalEndPoint.Port != PORT) && IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners().All(_ => _.Port != PORT))
            {
                ForceShutdown();
                return;
            }

            if (_restart)
                RestartThread();

            _shutdown = false;
            _isCompleted = false;

            _webthread = new Thread[5];
            _webqueue = new Queue<HttpListenerContext>();
            _webevent = new ManualResetEvent(false);
            _weblock = new object();

            WebSocket();

            Log.Info("Initializing AppEngine... OK!");
        }

        public static int ToMiliseconds(int minutes) => minutes * 60 * 1000;

        private void RestartThread()
        {
            Thread parallel_thread = new Thread(() =>
            {
                Thread.Sleep(ToMiliseconds(Settings.NETWORKING.RESTART.RESTART_APPENGINE_DELAY_MINUTES));

                int i = 5;
                do
                {
                    Log.Info($"AppEngine is restarting in {i} second{(i > 1 ? "s" : "")}...");
                    Thread.Sleep(1000);
                    i--;
                } while (i != 0);

                var webSocketIAsyncResult = new WebSocketDelegate(SafeShutdown).BeginInvoke(new AsyncCallback(SafeDispose), null);

                if (webSocketIAsyncResult.AsyncWaitHandle.WaitOne(5000, true))
                    Process.Start(Settings.APPENGINE.FILE);
            });

            parallel_thread.Start();
        }

        public bool SafeShutdown()
        {
            if (_webqueue.Count != 0)
                _webqueue.Clear();

            return _isCompleted = true;
        }

        public void SafeDispose(IAsyncResult webSocketIAsyncResult)
        {
            while (!_isCompleted)
                ;

            _websocket.Stop();
            _webevent.Set();

            AppEngine.GameData?.Dispose();
            AppEngine.Manager?.Dispose();

            Log.Warn("Terminated WebServer.");

            Thread.Sleep(1000);

            Environment.Exit(0);
        }

        private void ForceShutdown()
        {
            _shutdown = true;

            int i = 3;

            do
            {
                Log.Info($"Port {PORT} is occupied, restarting in {i} second{(i > 1 ? "s" : "")}...");
                Thread.Sleep(1000);
                i--;
            } while (i != 0);

            Log.Warn("Terminated AppEngine.");

            Thread.Sleep(1000);

            Process.Start(Settings.APPENGINE.FILE);

            Environment.Exit(0);
        }

        private void WebSocket()
        {
            string _webaddress = $"http://{(Settings.SERVER_MODE != Settings.ServerMode.Local ? "*" : "localhost")}:{PORT}/";

            WebSocketAddAddress(_webaddress, Environment.UserDomainName, Environment.UserName);

            _websocket = new HttpListener();
            _websocket.Prefixes.Add(_webaddress);
            _websocket.Start();

            try
            {
                _websocket.BeginGetContext(WebSocketCallback, null);
            }
            catch (Exception)
            {
                return;
            }

            int i = 0;

            do
            {
                _webthread[i] = new Thread(WebSocketThread)
                {
                    Name = $"WebSocketThread_{i}"
                };
                _webthread[i].Start();
                i++;
            } while (i < _webthread.Length);
        }

        private void WebSocketAddAddress(string address, string domain, string user)
        {
            string args = string.Format(@"http add urlacl url={0}", address) + " user=\"" + domain + "\\" + user + "\"";

            ProcessStartInfo psi = new ProcessStartInfo("netsh", args)
            {
                Verb = "runas",
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = true
            };

            Process.Start(psi).WaitForExit();
        }

        private void WebSocketCallback(IAsyncResult response)
        {
            try
            {
                if (!_websocket.IsListening)
                    return;

                HttpListenerContext _webcontext = _websocket.EndGetContext(response);

                _websocket.BeginGetContext(WebSocketCallback, null);

                lock (_weblock)
                {
                    _webqueue.Enqueue(_webcontext);
                    _webevent.Set();
                }
            }
            catch (Exception) { }
        }

        private void WebSocketThread()
        {
            do
            {
                if (_shutdown)
                    return;

                HttpListenerContext _webcontext;

                lock (_weblock)
                {
                    if (_webqueue.Count > 0)
                        _webcontext = _webqueue.Dequeue();
                    else
                    {
                        _webevent.Reset();
                        continue;
                    }
                }

                try
                {
                    WebSocketHandler(_webcontext);
                }
                catch (Exception)
                {
                    return;
                }
            } while (_webevent.WaitOne());
        }

        private void WebSocketHandler(HttpListenerContext _webcontext)
        {
            try
            {
                string _path;

                if (_webcontext.Request.Url.LocalPath.Contains("crossdomain"))
                {
                    new crossdomain().HandleRequest(_webcontext);
                    _webcontext.Response.Close();
                    return;
                }

                if (_webcontext.Request.Url.LocalPath.Contains("sfx") || _webcontext.Request.Url.LocalPath.Contains("music"))
                {
                    new Sfx().HandleRequest(_webcontext);
                    _webcontext.Response.Close();
                    return;
                }

                if (_webcontext.Request.Url.LocalPath.IndexOf(".") == -1)
                    _path = "LoESoft.AppEngine" + _webcontext.Request.Url.LocalPath.Replace("/", ".");
                else
                    _path = "LoESoft.AppEngine" + _webcontext.Request.Url.LocalPath.Remove(_webcontext.Request.Url.LocalPath.IndexOf(".")).Replace("/", ".");

                Type _type = Type.GetType(_path);

                if (_type != null)
                {
                    object _webhandler = Activator.CreateInstance(_type, null, null);

                    if (!(_webhandler is RequestHandler))
                    {
                        if (_webhandler == null)
                            using (var wtr = new StreamWriter(_webcontext.Response.OutputStream))
                                wtr.Write($"<Error>Class \"{_type.FullName}\" not found.</Error>");
                        else
                            using (var wtr = new StreamWriter(_webcontext.Response.OutputStream))
                                wtr.Write($"<Error>Class \"{_type.FullName}\" is not of the type RequestHandler.</Error>");
                    }
                    else
                        (_webhandler as RequestHandler).HandleRequest(_webcontext);
                    Log.Info($"[{(_webcontext.Request.RemoteEndPoint.Address.ToString() == "::1" ? "localhost" : $"{_webcontext.Request.RemoteEndPoint.Address.ToString()}")}] Request\t->\t{_webcontext.Request.Url.LocalPath}");
                }
                else
                    Log.Warn($"[{(_webcontext.Request.RemoteEndPoint.Address.ToString() == "::1" ? "localhost" : $"{_webcontext.Request.RemoteEndPoint.Address.ToString()}")}] Request\t->\t{_webcontext.Request.Url.LocalPath}");

                _webcontext.Response.Close();
            }
            catch (Exception)
            {
                if (_webqueue.Count != 0)
                    _webcontext = _webqueue.Dequeue();

                using (StreamWriter stream = new StreamWriter(_webcontext.Response.OutputStream))
                    stream.Write($"<h1>Bad request!</h1>\n{_webcontext.Request.Url.LocalPath}");
            }
        }
    }
}