using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;

public class NetworkMonitor
{
    public class AdapterInfo
    {
        public string Name;
        public PerformanceCounter Sent;
        public PerformanceCounter Received;
    }

    public List<AdapterInfo> Adapters = new List<AdapterInfo>();

    public NetworkMonitor()
    {
        foreach (var name in new PerformanceCounterCategory("Network Interface").GetInstanceNames())
        {
            if (name.ToLower().Contains("loopback") || name.ToLower().Contains("virtual"))
                continue;

            var adapter = new AdapterInfo
            {
                Name = name,
                Sent = new PerformanceCounter("Network Interface", "Bytes Sent/sec", name),
                Received = new PerformanceCounter("Network Interface", "Bytes Received/sec", name)
            };

            adapter.Sent.NextValue();
            adapter.Received.NextValue();

            Adapters.Add(adapter);
        }
    }

    public List<(string name, float upload, float download)> GetStats()
    {
        var result = new List<(string, float, float)>();

        foreach (var adapter in Adapters)
        {
            float upload = adapter.Sent.NextValue() / 1024f;
            float download = adapter.Received.NextValue() / 1024f;

            result.Add((adapter.Name, upload, download));
        }

        return result;
    }
    private string GetAktivesNetzwerk()
    {
        var category = new PerformanceCounterCategory("Network Interface");
        string[] instances = category.GetInstanceNames();

        foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (nic.OperationalStatus == OperationalStatus.Up &&
                nic.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                nic.Speed > 0 &&
                nic.GetIPProperties().GatewayAddresses.Count > 0)
            {
                // Debugging: Ausgabe der Netzwerkschnittstellen
                Debug.WriteLine($"Gefundene Netzwerkschnittstelle: {nic.Description}");

                // Normalisieren der Beschreibung für den Vergleich
                string normalizedDescription = nic.Description.Replace("(", "").Replace(")", "").Trim();
                foreach (var instance in instances)
                {
                    Debug.WriteLine($"Vergleiche: {normalizedDescription} mit {instance}");
                    if (instance.Contains(normalizedDescription))
                    {
                        Debug.WriteLine($"Aktives Netzwerk gefunden: {instance}");
                        return instance;
                    }
                }
            }
        }

        Debug.WriteLine("Kein aktives Netzwerk gefunden.");
        return null;
    }
}