using System.Collections.Concurrent;

namespace LoESoft.AppEngine
{
    public class WebClientMonitor
    {
        private ConcurrentDictionary<string, bool> WebClients { get; set; }

        public WebClientMonitor() => WebClients = new ConcurrentDictionary<string, bool>();

        public void AddWebClient(string ip, bool entry) => WebClients.TryAdd(ip, entry);

        public void UpdateWebClient(string ip, bool entry) => WebClients[ip] = entry;

        public bool CheckWebClient(string ip) => WebClients.ContainsKey(ip);
    }
}