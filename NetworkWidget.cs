using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace Temperatur
{
    public partial class NetworkWidget : Form
    {
        private Timer updateTimer;
        private Label infoLabel;
        private PerformanceCounter downloadCounter;
        private PerformanceCounter uploadCounter;

        public NetworkWidget()
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.StartPosition = FormStartPosition.Manual;
            this.BackColor = Color.Black;
            this.ForeColor = Color.Lime;
            this.Size = new Size(150, 50);
            this.ShowInTaskbar = false;

            infoLabel = new Label();
            infoLabel.Dock = DockStyle.Fill;
            infoLabel.TextAlign = ContentAlignment.MiddleCenter;
            infoLabel.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            infoLabel.ForeColor = Color.White;
            this.Controls.Add(infoLabel);

            PositioniereFensterAmTaskleistenrand();

            InitialisiereNetzwerkCounter();

            updateTimer = new Timer();
            updateTimer.Interval = 1000; // 1 Sekunde
            updateTimer.Tick += UpdateTimer_Tick;
            updateTimer.Start();
        }

        private void PositioniereFensterAmTaskleistenrand()
        {
            int x = Screen.PrimaryScreen.WorkingArea.Width - this.Width - 10;
            int y = Screen.PrimaryScreen.WorkingArea.Height - this.Height - 5;
            this.Location = new Point(x, y);
        }

        private void InitialisiereNetzwerkCounter()
        {
            string adapterName = GetAktivesNetzwerk();
            if (adapterName != null)
            {
                downloadCounter = new PerformanceCounter("Network Interface", "Bytes Received/sec", adapterName);
                uploadCounter = new PerformanceCounter("Network Interface", "Bytes Sent/sec", adapterName);

                // Erstes NextValue() aufrufen, um "Initialwert" zu holen
                downloadCounter.NextValue();
                uploadCounter.NextValue();
            }
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            if (downloadCounter == null || uploadCounter == null)
            {
                // Versuchen neu zu initialisieren
                InitialisiereNetzwerkCounter();

                if (downloadCounter == null || uploadCounter == null)
                {
                    // Immer noch kein Netz
                    infoLabel.Text = "Kein Netz";
                    return; // Frühzeitig rausgehen
                }
            }

            float download = downloadCounter.NextValue() / 1024f;
            float upload = uploadCounter.NextValue() / 1024f;

            infoLabel.Text = $"↓ {download:F0} KB/s\n↑ {upload:F0} KB/s";
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
}