
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibreHardwareMonitor.Hardware;
using System.Management;
using System.Drawing;
using System.Runtime.InteropServices;
using LiveCharts;
using LiveCharts.Wpf;
using System.Linq;

namespace Temperatur
{



    public partial class Form1 : Form
    {



        class Adapter
        {
            public string Name;
            public PerformanceCounter Sent;
            public PerformanceCounter Received;
        }


        private System.ComponentModel.IContainer components = null;
        private Computer computer;
        private PerformanceCounter ramCounter;
        private PerformanceCounter cpuCounter;
        private Label label1;
        private Timer timer1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Button btnSave;
        private Button btnSave2;
        private Label hardwareInfoLabelRam;
        private Label hardwareInfoLabelCpu;
        private Label hardwareInfoLabelGpu;
        private NumericUpDown numericUpDownCpu;
        private NumericUpDown numericUpDownGpu;
        private NetworkMonitor monitor;
        private ListBox listBox1;
        private NotifyIcon NetzwerkMonitor;
        private List<Adapter> adapters = new List<Adapter>();
        private CheckBox checkBoxProtokoll;
        private CheckBox checkBoxWidget;
        private CheckBox checkBoxGPU;
        private NetworkWidget widget;
        private SeriesCollection temperatureSeries;
        private LiveCharts.WinForms.CartesianChart temperatureChart;
        private LiveCharts.WinForms.CartesianChart GpuTempChart;
        private SeriesCollection gpuTempSeries;
        private LiveCharts.WinForms.CartesianChart networkChart;
        private SeriesCollection networkSeries;
        private LiveCharts.WinForms.CartesianChart ramusageChart;
        private SeriesCollection ramUsageSeries;





        //Cpu Temp Protokoll
        private Timer protokollTimer;
        private string protokollPfad = "cpu_temperatur_log.txt";
        //GPU Temp Protokoll
        private Timer protokollTimer2;
        private string protokollPfad2 = "gpu_temperatur_log.txt";

        public Form1()
        {
            InitializeComponent();
            ApplySystemTheme();
            WatchForThemeChanges();
            InitializeTemperatureGraph();
            InitializeGpuTempGraph();
            InitializeRamUsageGraph();
            InitNetwork();
            InitializeNetworkGraph();

            StartMonitoring();
            monitor = new NetworkMonitor();
            StartMonitoring();
            computer = new Computer()
            {
                IsCpuEnabled = true,//CPU-Überwachung aktivieren
                IsGpuEnabled = true //GPU-Überwachung aktivieren
            };
            computer.Open();
        }
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.hardwareInfoLabelRam = new System.Windows.Forms.Label();
            this.hardwareInfoLabelCpu = new System.Windows.Forms.Label();
            this.hardwareInfoLabelGpu = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.NetzwerkMonitor = new System.Windows.Forms.NotifyIcon(this.components);
            this.checkBoxProtokoll = new System.Windows.Forms.CheckBox();
            this.checkBoxWidget = new System.Windows.Forms.CheckBox();
            this.checkBoxGPU = new System.Windows.Forms.CheckBox();
            this.numericUpDownCpu = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownGpu = new System.Windows.Forms.NumericUpDown();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnSave2 = new System.Windows.Forms.Button();
            this.temperatureChart = new LiveCharts.WinForms.CartesianChart();
            this.networkChart = new LiveCharts.WinForms.CartesianChart();
            this.GpuTempChart = new LiveCharts.WinForms.CartesianChart();
            this.ramusageChart = new LiveCharts.WinForms.CartesianChart();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCpu)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownGpu)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "CPU Temperatur: ";
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 97);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(107, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "CPU Auslastung:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(566, 51);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 16);
            this.label3.TabIndex = 2;
            this.label3.Text = "RAM:";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(21, 323);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(115, 16);
            this.label4.TabIndex = 4;
            this.label4.Text = "GPU Temperatur: ";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(21, 367);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(111, 16);
            this.label5.TabIndex = 5;
            this.label5.Text = "GPU Auslastung: ";
            // 
            // hardwareInfoLabelRam
            // 
            this.hardwareInfoLabelRam.AutoSize = true;
            this.hardwareInfoLabelRam.Location = new System.Drawing.Point(566, 9);
            this.hardwareInfoLabelRam.Name = "hardwareInfoLabelRam";
            this.hardwareInfoLabelRam.Size = new System.Drawing.Size(178, 16);
            this.hardwareInfoLabelRam.TabIndex = 0;
            this.hardwareInfoLabelRam.Text = "Standartext: Ram-Information";
            // 
            // hardwareInfoLabelCpu
            // 
            this.hardwareInfoLabelCpu.AutoSize = true;
            this.hardwareInfoLabelCpu.Location = new System.Drawing.Point(18, 9);
            this.hardwareInfoLabelCpu.Name = "hardwareInfoLabelCpu";
            this.hardwareInfoLabelCpu.Size = new System.Drawing.Size(200, 16);
            this.hardwareInfoLabelCpu.TabIndex = 0;
            this.hardwareInfoLabelCpu.Text = "Standardtext: CPU-Informationen";
            // 
            // hardwareInfoLabelGpu
            // 
            this.hardwareInfoLabelGpu.AutoSize = true;
            this.hardwareInfoLabelGpu.Location = new System.Drawing.Point(18, 279);
            this.hardwareInfoLabelGpu.Name = "hardwareInfoLabelGpu";
            this.hardwareInfoLabelGpu.Size = new System.Drawing.Size(201, 16);
            this.hardwareInfoLabelGpu.TabIndex = 0;
            this.hardwareInfoLabelGpu.Text = "Standardtext: GPU-Informationen";
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 16;
            this.listBox1.Location = new System.Drawing.Point(566, 279);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(160, 84);
            this.listBox1.TabIndex = 3;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // NetzwerkMonitor
            // 
            this.NetzwerkMonitor.Text = "notifyIcon1";
            this.NetzwerkMonitor.Visible = true;
            // 
            // checkBoxProtokoll
            // 
            this.checkBoxProtokoll.Location = new System.Drawing.Point(21, 141);
            this.checkBoxProtokoll.Name = "checkBoxProtokoll";
            this.checkBoxProtokoll.Size = new System.Drawing.Size(111, 19);
            this.checkBoxProtokoll.TabIndex = 0;
            this.checkBoxProtokoll.Text = "CPU-Protokoll";
            this.checkBoxProtokoll.CheckedChanged += new System.EventHandler(this.checkBoxProtokoll_CheckedChanged_1);
            // 
            // checkBoxWidget
            // 
            this.checkBoxWidget.Location = new System.Drawing.Point(569, 451);
            this.checkBoxWidget.Name = "checkBoxWidget";
            this.checkBoxWidget.Size = new System.Drawing.Size(145, 38);
            this.checkBoxWidget.TabIndex = 0;
            this.checkBoxWidget.Text = "Netzwerk Widget";
            this.checkBoxWidget.CheckedChanged += new System.EventHandler(this.checkBoxWidget_CheckedChanged_1);
            // 
            // checkBoxGPU
            // 
            this.checkBoxGPU.Location = new System.Drawing.Point(21, 411);
            this.checkBoxGPU.Name = "checkBoxGPU";
            this.checkBoxGPU.Size = new System.Drawing.Size(145, 38);
            this.checkBoxGPU.TabIndex = 2;
            this.checkBoxGPU.Text = "GPU-Protokoll";
            this.checkBoxGPU.CheckedChanged += new System.EventHandler(this.checkBoxWidget_CheckedChanged_1);
            // 
            // numericUpDownCpu
            // 
            this.numericUpDownCpu.Location = new System.Drawing.Point(21, 161);
            this.numericUpDownCpu.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.numericUpDownCpu.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownCpu.Name = "numericUpDownCpu";
            this.numericUpDownCpu.Size = new System.Drawing.Size(120, 22);
            this.numericUpDownCpu.TabIndex = 0;
            this.numericUpDownCpu.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // numericUpDownGpu
            // 
            this.numericUpDownGpu.Location = new System.Drawing.Point(21, 441);
            this.numericUpDownGpu.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.numericUpDownGpu.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownGpu.Name = "numericUpDownGpu";
            this.numericUpDownGpu.Size = new System.Drawing.Size(120, 22);
            this.numericUpDownGpu.TabIndex = 1;
            this.numericUpDownGpu.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(21, 186);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(120, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Speichern";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnSave2
            // 
            this.btnSave2.Location = new System.Drawing.Point(21, 466);
            this.btnSave2.Name = "btnSave2";
            this.btnSave2.Size = new System.Drawing.Size(120, 23);
            this.btnSave2.TabIndex = 3;
            this.btnSave2.Text = "Speichern";
            this.btnSave2.UseVisualStyleBackColor = true;
            this.btnSave2.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // temperatureChart
            // 
            this.temperatureChart.Location = new System.Drawing.Point(190, 51);
            this.temperatureChart.Name = "temperatureChart";
            this.temperatureChart.Size = new System.Drawing.Size(370, 158);
            this.temperatureChart.TabIndex = 6;
            // 
            // networkChart
            // 
            this.networkChart.Location = new System.Drawing.Point(735, 323);
            this.networkChart.Name = "networkChart";
            this.networkChart.Size = new System.Drawing.Size(370, 158);
            this.networkChart.TabIndex = 7;
            // 
            // GpuTempChart
            // 
            this.GpuTempChart.Location = new System.Drawing.Point(190, 331);
            this.GpuTempChart.Name = "GpuTempChart";
            this.GpuTempChart.Size = new System.Drawing.Size(370, 158);
            this.GpuTempChart.TabIndex = 6;
            // 
            // ramusageChart
            // 
            this.ramusageChart.Location = new System.Drawing.Point(735, 51);
            this.ramusageChart.Name = "ramusageChart";
            this.ramusageChart.Size = new System.Drawing.Size(370, 158);
            this.ramusageChart.TabIndex = 6;
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(1143, 536);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnSave2);
            this.Controls.Add(this.temperatureChart);
            this.Controls.Add(this.GpuTempChart);
            this.Controls.Add(this.numericUpDownGpu);
            this.Controls.Add(this.numericUpDownCpu);
            this.Controls.Add(this.checkBoxProtokoll);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.checkBoxWidget);
            this.Controls.Add(this.checkBoxGPU);
            this.Controls.Add(this.hardwareInfoLabelRam);
            this.Controls.Add(this.hardwareInfoLabelCpu);
            this.Controls.Add(this.hardwareInfoLabelGpu);
            this.Controls.Add(this.networkChart);
            this.Controls.Add(this.ramusageChart);
            this.Name = "Form1";
            this.Text = "Hardware Monitor";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCpu)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownGpu)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void InitializeRamUsageGraph()
        {
            // Initialisiere die Datenreihen
            ramUsageSeries = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "RAM Nutzung",
                    Values = new ChartValues<float>(),
                    PointGeometry = DefaultGeometries.Circle,
                    PointGeometrySize = 5
                }
            };

            // Erstelle den RAM-Nutzungs-Graphen
            ramusageChart.Series = ramUsageSeries;
            ramusageChart.AxisX.Add(new Axis
            {
                Title = "Zeit",
                Labels = new List<string>() // Zeitstempel werden hier hinzugefügt
            });

            // Konfiguriere die Y-Achse für RAM-Nutzung
            ramusageChart.AxisY.Add(new Axis
            {
                Title = "RAM Nutzung (GB)",
                LabelFormatter = value => value.ToString("F1")
            });

            // Timer für die Aktualisierung der Daten
            Timer ramUsageUpdateTimer = new Timer
            {
                Interval = 1000 // Aktualisierung alle 1 Sekunde
            };

            ramUsageUpdateTimer.Tick += (sender, e) =>
            {
                // RAM-Nutzung abrufen
                float totalRam = new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory / (1024 * 1024 * 1024);
                float availableRam = ramCounter.NextValue() / 1024; // In GB umrechnen
                float usedRam = totalRam - availableRam;

                // Werte zum Graphen hinzufügen
                ramUsageSeries[0].Values.Add(usedRam);

                // Begrenze die Anzahl der Punkte im Graphen
                if (ramUsageSeries[0].Values.Count > 50)
                {
                    ramUsageSeries[0].Values.RemoveAt(0);
                }

                // Zeitstempel hinzufügen
                var labels = ramusageChart.AxisX[0].Labels;
                labels.Add(DateTime.Now.ToString("HH:mm:ss"));
                if (labels.Count() > 50)
                {
                    labels.RemoveAt(0);
                }
            };

            ramUsageUpdateTimer.Start();
        }
        private void InitializeGpuTempGraph()
        {
            // Initialisiere die Datenreihen
            gpuTempSeries = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "GPU Temperatur",
                    Values = new ChartValues<float>(),
                    PointGeometry = DefaultGeometries.Circle,
                    PointGeometrySize = 5
                }
            };

            GpuTempChart.Series = gpuTempSeries;

            GpuTempChart.AxisX.Add(new Axis
            {
                Title = "Zeit",
                Labels = new List<string>() // Zeitstempel werden hier hinzugefügt
            });

            GpuTempChart.AxisY.Add(new Axis
            {
                Title = "GPU Temperatur (°C)",
                LabelFormatter = value => value.ToString("F1")
            });

            // Timer für die Aktualisierung der Daten
            Timer gpuTempUpdateTimer = new Timer
            {
                Interval = 1000 // Aktualisierung alle 1 Sekunde
            };

            gpuTempUpdateTimer.Tick += (sender, e) =>
            {
                // GPU-Temperatur abrufen
                float? gpuTemp = GetGpuTemperature();

                // Fallback auf CPU-Temperatur, falls GPU-Temperatur nicht verfügbar ist
                float? tempToDisplay = gpuTemp ?? GetCpuTemperature();

                if (tempToDisplay.HasValue)
                {
                    gpuTempSeries[0].Values.Add(tempToDisplay.Value);

                    // Begrenze die Anzahl der Punkte im Graphen
                    if (gpuTempSeries[0].Values.Count > 50)
                    {
                        gpuTempSeries[0].Values.RemoveAt(0);
                    }

                    // Zeitstempel hinzufügen
                    GpuTempChart.AxisX[0].Labels.Add(DateTime.Now.ToString("HH:mm:ss"));
                    if (GpuTempChart.AxisX[0].Labels.Count > 50)
                    {
                        GpuTempChart.AxisX[0].Labels.RemoveAt(0);
                    }
                }
            };

            gpuTempUpdateTimer.Start();
        }
        private void InitializeTemperatureGraph()
        {
            // Initialisiere die Datenreihen
            temperatureSeries = new SeriesCollection
            {

                new LineSeries
                {
                Title = "CPU Temperatur",
                Values = new ChartValues<float>()
                },

            };
            temperatureChart.Series = temperatureSeries;
            temperatureChart.AxisX.Add(new Axis
            {
                Title = "Zeit",
                Labels = new List<string>() // Zeitstempel werden hier hinzugefügt
            });
            temperatureChart.AxisY.Add(new Axis
            {
                Title = "Temperatur (°C)",
                LabelFormatter = value => value.ToString("F1")
            });
        }



        // Konfiguriere die Achsen

        // Systemdesign ändern
        private void WatchForThemeChanges()
        {
            Microsoft.Win32.SystemEvents.UserPreferenceChanged += (s, e) =>
            {
                if (e.Category == Microsoft.Win32.UserPreferenceCategory.General)
                {
                    ApplySystemTheme();
                }
            };
        }
        //DWM API für dunklen Modus
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        private void ApplySystemTheme()
        {

            // Prüfen, ob Dark Mode aktiv ist
            bool isDarkMode = IsSystemInDarkMode();

            // Titelleiste anpassen
            int useDarkMode = isDarkMode ? 1 : 0;
            DwmSetWindowAttribute(this.Handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref useDarkMode, sizeof(int));

            // Hintergrund- und Textfarben für das Formular
            this.BackColor = isDarkMode ? Color.FromArgb(30, 30, 30) : Color.White;
            this.ForeColor = isDarkMode ? Color.White : Color.Black;

            // Labels
            label1.ForeColor = isDarkMode ? Color.White : Color.Black;
            label2.ForeColor = isDarkMode ? Color.White : Color.Black;
            label3.ForeColor = isDarkMode ? Color.White : Color.Black;
            label4.ForeColor = isDarkMode ? Color.White : Color.Black;
            label5.ForeColor = isDarkMode ? Color.White : Color.Black;
            hardwareInfoLabelRam.ForeColor = isDarkMode ? Color.White : Color.Black;
            hardwareInfoLabelCpu.ForeColor = isDarkMode ? Color.White : Color.Black;
            hardwareInfoLabelGpu.ForeColor = isDarkMode ? Color.White : Color.Black;

            // ListBox
            listBox1.BackColor = isDarkMode ? Color.FromArgb(45, 45, 45) : Color.White;
            listBox1.ForeColor = isDarkMode ? Color.White : Color.Black;

            // CheckBoxes
            checkBoxProtokoll.ForeColor = isDarkMode ? Color.White : Color.Black;
            checkBoxWidget.ForeColor = isDarkMode ? Color.White : Color.Black;
            checkBoxGPU.ForeColor = isDarkMode ? Color.White : Color.Black;

            // NumericUpDown
            numericUpDownCpu.BackColor = isDarkMode ? Color.FromArgb(45, 45, 45) : Color.White;
            numericUpDownCpu.ForeColor = isDarkMode ? Color.White : Color.Black;
            numericUpDownGpu.BackColor = isDarkMode ? Color.FromArgb(45, 45, 45) : Color.White;
            numericUpDownGpu.ForeColor = isDarkMode ? Color.White : Color.Black;

            // Button
            btnSave.BackColor = isDarkMode ? Color.FromArgb(45, 45, 45) : Color.White;
            btnSave.ForeColor = isDarkMode ? Color.White : Color.Black;
            // btnSave2
            btnSave2.BackColor = isDarkMode ? Color.FromArgb(45, 45, 45) : Color.White;
            btnSave2.ForeColor = isDarkMode ? Color.White : Color.Black;
        }

        private bool IsSystemInDarkMode()
        {
            try
            {
                using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
                {
                    if (key != null)
                    {
                        object value = key.GetValue("AppsUseLightTheme");
                        if (value != null && (int)value == 0)
                        {
                            return true; // Dark Mode ist aktiviert
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Fehler beim Abfragen des Systemdesigns: {ex.Message}");
            }
            return false; // Standard: Light Mode
        }






        //Netzwerk initialisieren
        private void InitNetwork()
        {
            string[] names = new PerformanceCounterCategory("Network Interface").GetInstanceNames();

            foreach (string name in names)
            {
                if (name.ToLower().Contains("loopback") || name.ToLower().Contains("virtual"))
                    continue;

                Adapter a = new Adapter();
                a.Name = name;
                a.Sent = new PerformanceCounter("Network Interface", "Bytes Sent/sec", name);
                a.Received = new PerformanceCounter("Network Interface", "Bytes Received/sec", name);
                adapters.Add(a);


            }

            foreach (Adapter a in adapters)
            {
                a.Sent.NextValue();
                a.Received.NextValue();
            }
        }
        //Netzwerkmonitor starten
        private async void StartMonitoring()
        {
            while (true)
            {

                float maxUsage = 0;
                string activeAdapter = "";
                float up = 0, down = 0;

                foreach (Adapter a in adapters)
                {
                    float sent = a.Sent.NextValue() / 1024f;
                    float recv = a.Received.NextValue() / 1024f;
                    float sum = sent + recv;

                    if (sum > maxUsage)
                    {
                        maxUsage = sum;
                        activeAdapter = a.Name;
                        up = sent;
                        down = recv;


                    }
                }

                string display = $"{down:F0}/{up:F0}";


                int topIndex = listBox1.TopIndex;
                listBox1.BeginUpdate();
                listBox1.Items.Clear();
                listBox1.Items.Add(activeAdapter);
                listBox1.Items.Add(" Upload: " + up.ToString("F2") + " KB/s");
                listBox1.Items.Add(" Download: " + down.ToString("F2") + " KB/s");
                listBox1.Items.Add("");
                listBox1.EndUpdate();
                listBox1.TopIndex = topIndex;

                networkSeries[0].Values.Add(up);
                networkSeries[1].Values.Add(down);

                if (networkSeries[0].Values.Count > 50)
                {
                    networkSeries[0].Values.RemoveAt(0);
                    networkSeries[1].Values.RemoveAt(0);
                }

                await Task.Delay(1000);
            }

        }
        //Netzwerk



        private void Form1_Load(object sender, EventArgs e)
        {
            ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            // CPU Counter initialisieren und erste Werte abrufen
            cpuCounter.NextValue(); // Initialisieren
            timer1.Start();

            //Hardware Informationen laden
            LadeHardwareInformationen();
            // CPU Temp Protokoll Timer
            protokollTimer = new Timer();
            protokollTimer.Interval = 10 * 1000;
            protokollTimer.Tick += ProtokollTimer_Tick;
            protokollTimer.Start();

            // GPU Temp Protokoll Timer
            protokollTimer2 = new Timer();
            protokollTimer2.Interval = 10 * 1000;
            protokollTimer2.Tick += ProtokollTimer2_Tick;
            protokollTimer2.Start();

            //CheckBox Protokoll
            checkBoxProtokoll.CheckedChanged += CheckBoxProtokoll_CheckedChanged;
            //CheckboxWidget
            checkBoxWidget.CheckedChanged += CheckBoxWidget_CheckedChanged;
            //CheckBox GPU
            checkBoxGPU.CheckedChanged += CheckBoxWidget_CheckedChanged;


            var widget = new NetworkWidget();
            widget = null;

        }
        //Hardware erkennung

        private void LadeHardwareInformationen()
        {
            try
            {
                string hardwareInfo1 = "";

                // CPU-Informationen
                var cpuQuery = new ManagementObjectSearcher("select * from Win32_Processor");
                foreach (var obj in cpuQuery.Get())
                {
                    hardwareInfo1 += $"CPU: {obj["Name"]}\n";
                }
                hardwareInfoLabelCpu.Text = hardwareInfo1;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Fehler beim Abrufen der CPU-Informationen: {ex.Message}");
                hardwareInfoLabelCpu.Text = "CPU-Informationen nicht verfügbar.";
            }

            try
            {
                string hardwareInfo2 = "";

                // RAM-Informationen nach Slots
                var ramQuery = new ManagementObjectSearcher("select * from Win32_PhysicalMemory");
                int slotNumber = 1;
                foreach (var obj in ramQuery.Get())
                {
                    string capacity = $"{Math.Round(Convert.ToDouble(obj["Capacity"]) / (1024 * 1024 * 1024), 2)} GB";
                    string speed = obj["Speed"] != null ? $"{obj["Speed"]} MHz" : "Unbekannte Geschwindigkeit";
                    string manufacturer = obj["Manufacturer"] != null ? obj["Manufacturer"].ToString() : "Unbekannter Hersteller";

                    hardwareInfo2 += $"Slot {slotNumber}: {capacity}, {speed}, {manufacturer}\n";
                    slotNumber++;
                }
                hardwareInfoLabelRam.Text = hardwareInfo2;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Fehler beim Abrufen der RAM-Informationen: {ex.Message}");
                hardwareInfoLabelRam.Text = "RAM-Informationen nicht verfügbar.";
            }

            try
            {
                string hardwareInfo3 = "";

                // GPU-Informationen
                var gpuQuery = new ManagementObjectSearcher("select * from Win32_VideoController");
                foreach (var obj in gpuQuery.Get())
                {
                    hardwareInfo3 += $"GPU: {obj["Name"]}\n";
                }
                hardwareInfoLabelGpu.Text = hardwareInfo3;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Fehler beim Abrufen der GPU-Informationen: {ex.Message}");
                hardwareInfoLabelGpu.Text = "GPU-Informationen nicht verfügbar.";
            }
        }

        //Aktivierung für Cpu Temp Protokoll

        private void CheckBoxProtokoll_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxProtokoll.Checked)
            {
                protokollTimer.Start();
            }
            else
            {
                protokollTimer.Stop();
            }

        }
        //Aktivierung für GPU Temp Protokoll

        private void CheckBoxGPU_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxGPU.Checked)
            {
                protokollTimer2.Start();
            }
            else
            {
                protokollTimer2.Stop();
            }
        }

        private void ProtokollTimer2_Tick(object sender, EventArgs e)
        {
            float? gpuTemperatur = GetGpuTemperature();
            if (gpuTemperatur.HasValue)
            {
                string eintrag = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {gpuTemperatur.Value:F1} °C";
                System.IO.File.AppendAllText(protokollPfad2, eintrag + Environment.NewLine);
            }
        }
        //CPU Temp Protokoll eintrag
        private void ProtokollTimer_Tick(object sender, EventArgs e)
        {
            float? temperatur = GetCpuTemperature();
            if (temperatur.HasValue)
            {
                string eintrag = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {temperatur.Value:F1} °C";
                System.IO.File.AppendAllText(protokollPfad, eintrag + Environment.NewLine);
            }
        }

        //Aktivierung für Netzwerk Widget
        private void CheckBoxWidget_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxWidget.Checked)
            {
                widget = new NetworkWidget();
                widget.Show();
            }
            else
            {
                if (widget != null && !widget.IsDisposed)
                {
                    widget.Close();
                }
            }
        }

        //Timer
        private void timer1_Tick(object sender, EventArgs e)
        {
            // Debugging: Gibt den aktuellen Wert aus, um zu sehen, ob er funktioniert.
            Console.WriteLine("Timer Ticked");

            // CPU Temperatur anzeigen
            float? temp = GetCpuTemperature();
            if (temp.HasValue)
            {
                label1.Text = $"CPU Temperatur: {temp.Value:F1} °C";
            }
            else
            {
                label1.Text = "Temperatur nicht verfügbar.";
            }
            // CPU-Auslastung (wird als Prozentwert angezeigt)
            float cpuUsage = cpuCounter.NextValue();


            label2.Text = $"CPU Auslastung: {cpuUsage:F1} %";

            //GPU Temperatur anzeigen
            float? gpuTemp = GetGpuTemperature();
            if (gpuTemp.HasValue)
            {
                label4.Text = $"GPU Temperatur: {gpuTemp.Value:F1} °C";
            }
            else
            {
                label4.Text = $"GPU Temperatur: {temp.Value:F1} °C";
            }


            //GPU-Auslastung (wird als Prozentwert angezeigt)
            float gpuUsage = cpuCounter.NextValue();

            label5.Text = $"GPU Auslastung: {gpuUsage:F1} %";



            // Verfügbaren RAM anzeigen

            var totalRam = new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory / (1024 * 1024 * 1024);
            var availableRam = ramCounter.NextValue();
            var usedRam = totalRam - (availableRam / 1024);
            label3.Text = $"Ram: {usedRam:F0} GB von {totalRam:F0} GB";


            // Debugging-Ausgabe: Überprüfe, ob Werte zurückgegeben werden.
            Console.WriteLine($"CPU Usage: {cpuUsage}, RAM: {availableRam}");
            // CPU Temperatur abrufen
            float? cpuTemp = GetCpuTemperature();
            if (cpuTemp.HasValue)
            {
                temperatureSeries[0].Values.Add(cpuTemp.Value);
            }

            // GPU Temperatur abrufen
            float? gpuTempForGraph = GetGpuTemperature();
            if (gpuTemp.HasValue)
            {
                temperatureSeries[1].Values.Add(gpuTemp.Value);
            }



            // Debugging-Ausgabe
            Console.WriteLine($"CPU: {cpuTemp} °C, GPU: {gpuTemp} °C");
        }

        //GPU Temperatur abfragen
        private float? GetGpuTemperature()
        {
            foreach (var hardwareItem in computer.Hardware)
            {
                if (hardwareItem.HardwareType == HardwareType.GpuNvidia || hardwareItem.HardwareType == HardwareType.GpuAmd || hardwareItem.HardwareType == HardwareType.GpuIntel)
                {
                    // GPU-Temperatur abfragen
                    hardwareItem.Update();
                    foreach (var sensor in hardwareItem.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Temperature && sensor.Value.HasValue)
                        {
                            return sensor.Value.Value;
                        }
                    }
                }

            }
            return null;
        }

        //CPU Temperatur abfragen
        private float? GetCpuTemperature()
        {
            foreach (var hardwareItem in computer.Hardware)
            {
                if (hardwareItem.HardwareType == HardwareType.Cpu)
                {
                    hardwareItem.Update();
                    foreach (var sensor in hardwareItem.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Temperature && sensor.Value.HasValue)
                        {
                            return sensor.Value.Value;
                        }
                    }
                }
            }

            return null;
        }

        private void checkBoxWidget_CheckedChanged_1(object sender, EventArgs e)
        {

        }

        private void checkBoxProtokoll_CheckedChanged_1(object sender, EventArgs e)
        {

        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            // Speichern der aktuellen Werte aus den NumericUpDown-Steuerelementen
            int cpuInterval = (int)numericUpDownCpu.Value * 1000; // Umrechnung in Millisekunden
            int gpuInterval = (int)numericUpDownGpu.Value * 1000; // Umrechnung in Millisekunden

            // Aktualisiere die Timer-Intervalle
            protokollTimer.Interval = cpuInterval;
            protokollTimer2.Interval = gpuInterval;

            MessageBox.Show("Einstellungen gespeichert!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void btnSettings_Click(object sender, EventArgs e)
        {
            // Öffne das Einstellungsmenü mit den aktuellen Intervallen
            using (var settingsForm = new SettingsForm(protokollTimer.Interval, protokollTimer2.Interval))
            {
                if (settingsForm.ShowDialog() == DialogResult.OK)
                {
                    // Aktualisiere die Timer-Intervalle
                    protokollTimer.Interval = settingsForm.CpuTickInterval;
                    protokollTimer2.Interval = settingsForm.GpuTickInterval;
                }
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
        private void InitializeNetworkGraph()
        {
            // Initialisiere die Datenreihen
            networkSeries = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Upload (KB/s)",
                    Values = new ChartValues<float>()
                },
                new LineSeries
                {
                    Title = "Download (KB/s)",
                    Values = new ChartValues<float>()
                }
            };

            networkChart.Series = networkSeries;
            networkChart.AxisX.Add(new Axis
            {
                Title = "Zeit",
                Labels = new List<string>() // Zeitstempel werden hier hinzugefügt
            });
            networkChart.AxisY.Add(new Axis
            {
                Title = "Geschwindigkeit (KB/s)",
                LabelFormatter = value => value.ToString("F1")
            });
        }
    }
}
        