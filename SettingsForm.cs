// Ensure the SettingsForm class inherits from Form
using System.Windows.Forms;
using System;

public class SettingsForm : Form
{
    public int CpuTickInterval { get; private set; }
    public int GpuTickInterval { get; private set; }

    public SettingsForm(int cpuInterval, int gpuInterval)
    {
        CpuTickInterval = cpuInterval;
        GpuTickInterval = gpuInterval;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.numericUpDownCpu = new NumericUpDown();
        this.numericUpDownGpu = new NumericUpDown();
        this.btnSave = new Button();
        this.SuspendLayout();

        // numericUpDownCpu
        this.numericUpDownCpu.Location = new System.Drawing.Point(12, 12);
        this.numericUpDownCpu.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
        this.numericUpDownCpu.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        this.numericUpDownCpu.Name = "numericUpDownCpu";
        this.numericUpDownCpu.Size = new System.Drawing.Size(120, 22);
        this.numericUpDownCpu.TabIndex = 0;
        this.numericUpDownCpu.Value = new decimal(new int[] { CpuTickInterval / 1000, 0, 0, 0 });

        // numericUpDownGpu
        this.numericUpDownGpu.Location = new System.Drawing.Point(12, 40);
        this.numericUpDownGpu.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
        this.numericUpDownGpu.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        this.numericUpDownGpu.Name = "numericUpDownGpu";
        this.numericUpDownGpu.Size = new System.Drawing.Size(120, 22);
        this.numericUpDownGpu.TabIndex = 1;
        this.numericUpDownGpu.Value = new decimal(new int[] { GpuTickInterval / 1000, 0, 0, 0 });

        // btnSave
        this.btnSave.Location = new System.Drawing.Point(12, 68);
        this.btnSave.Name = "btnSave";
        this.btnSave.Size = new System.Drawing.Size(120, 23);
        this.btnSave.TabIndex = 2;
        this.btnSave.Text = "Speichern";
        this.btnSave.UseVisualStyleBackColor = true;
        this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

        // SettingsForm
        this.ClientSize = new System.Drawing.Size(150, 100);
        this.Controls.Add(this.btnSave);
        this.Controls.Add(this.numericUpDownGpu);
        this.Controls.Add(this.numericUpDownCpu);
        this.Name = "SettingsForm";
        this.Text = "Einstellungen";
        this.ResumeLayout(false);
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
        CpuTickInterval = (int)numericUpDownCpu.Value * 1000;
        GpuTickInterval = (int)numericUpDownGpu.Value * 1000;
        this.DialogResult = DialogResult.OK;
        this.Close();
    }

    private NumericUpDown numericUpDownCpu;
    private NumericUpDown numericUpDownGpu;
    private Button btnSave;
}