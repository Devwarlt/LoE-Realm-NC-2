#region

using LoESoft.AppEngine.account;
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
        public int PORT => Settings.APPENGINE.PRODUCTION_PORT;

        public bool _shutdown { get; set; }
        private bool _isCompleted { get; set; }
        private bool _restart { get; set; }

        private readonly List<HttpListenerContext> currentRequests = new List<HttpListenerContext>();
        private HttpListener listener;

        public delegate bool WebSocketDelegate();

        public AppEngineManager(bool restart)
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

            string url = $"http://{(Settings.SERVER_MODE != Settings.ServerMode.Local ? "*" : "localhost")}:{PORT}/";

            Process.Start(new ProcessStartInfo("netsh",
                string.Format(@"http add urlacl url={0}", url) +
                " user=\"" + Environment.UserDomainName + "\\" +
                Environment.UserName + "\"")
            {
                Verb = "runas",
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = true
            }).WaitForExit();

            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();

            listener.BeginGetContext(ListenerCallback, null);

            Log.Info("Initializing AppEngine... OK!");
        }

        private void ListenerCallback(IAsyncResult ar)
        {
            try
            {
                if (!listener.IsListening)
                    return;

                var context = listener.EndGetContext(ar);
                listener.BeginGetContext(ListenerCallback, null);

                HandleRequest(context);
            }
            catch { }
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

                var webSocketIAsyncResult = new WebSocketDelegate(() => true).BeginInvoke(new AsyncCallback(SafeDispose), null);

                if (webSocketIAsyncResult.AsyncWaitHandle.WaitOne(5000, true))
                    Process.Start(Settings.APPENGINE.FILE);
            });

            parallel_thread.Start();
        }

        public void SafeDispose(IAsyncResult webSocketIAsyncResult)
        {
            while (!_isCompleted)
                ;

            listener.Stop();

            AppEngine.GameData?.Dispose();

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

        private void HandleRequest(HttpListenerContext context)
        {
            try
            {
                string _path;

                if (context.Request.Url.LocalPath.Contains("admin/restart"))
                {
                    new restart().HandleRequest(context);
                    context.Response.Close();
                    return;
                }

                if (context.Request.Url.LocalPath.Contains("sfx") || context.Request.Url.LocalPath.Contains("music"))
                {
                    new Sfx().HandleRequest(context);
                    context.Response.Close();
                    return;
                }

                if (context.Request.Url.LocalPath.IndexOf(".") == -1)
                    _path = "LoESoft.AppEngine" + context.Request.Url.LocalPath.Replace("/", ".");
                else
                    _path = "LoESoft.AppEngine" + context.Request.Url.LocalPath.Remove(context.Request.Url.LocalPath.IndexOf(".")).Replace("/", ".");

                var _type = Type.GetType(_path);

                if (_type != null)
                {
                    var _webhandler = Activator.CreateInstance(_type, null, null);

                    if (!(_webhandler is RequestHandler))
                    {
                        if (_webhandler == null)
                            using (var wtr = new StreamWriter(context.Response.OutputStream))
                                wtr.Write($"<Error>Class \"{_type.FullName}\" not found.</Error>");
                        else
                            using (var wtr = new StreamWriter(context.Response.OutputStream))
                                wtr.Write($"<Error>Class \"{_type.FullName}\" is not of the type RequestHandler.</Error>");
                    }
                    else
                        (_webhandler as RequestHandler).HandleRequest(context);

                    Log.Info($"[{(context.Request.RemoteEndPoint.Address.ToString() == "::1" ? "localhost" : $"{context.Request.RemoteEndPoint.Address.ToString()}")}] Request\t->\t{context.Request.Url.LocalPath}");
                }
                else
                    Log.Warn($"[{(context.Request.RemoteEndPoint.Address.ToString() == "::1" ? "localhost" : $"{context.Request.RemoteEndPoint.Address.ToString()}")}] Request\t->\t{context.Request.Url.LocalPath}");

                context.Response.Close();
            }
            catch { currentRequests.Remove(context); }

            context.Response.Close();
        }
    }
}