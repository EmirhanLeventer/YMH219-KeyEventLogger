namespace KeyEventLogger;

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        pnlTop = new FlowLayoutPanel();
        btnStart = new Button();
        btnStop = new Button();
        btnClear = new Button();
        btnSave = new Button();
        lblStatus = new Label();
        lstLog = new ListBox();
        lblCount = new Label();
        pnlTop.SuspendLayout();
        SuspendLayout();
        //
        // pnlTop
        //
        pnlTop.AutoSize = true;
        pnlTop.Controls.Add(btnStart);
        pnlTop.Controls.Add(btnStop);
        pnlTop.Controls.Add(btnClear);
        pnlTop.Controls.Add(btnSave);
        pnlTop.Controls.Add(lblStatus);
        pnlTop.Dock = DockStyle.Top;
        pnlTop.Location = new Point(0, 0);
        pnlTop.Name = "pnlTop";
        pnlTop.Padding = new Padding(8);
        pnlTop.Size = new Size(760, 58);
        pnlTop.TabIndex = 0;
        pnlTop.WrapContents = false;
        //
        // btnStart
        //
        btnStart.Location = new Point(11, 11);
        btnStart.Name = "btnStart";
        btnStart.Size = new Size(110, 36);
        btnStart.TabIndex = 0;
        btnStart.TabStop = false;
        btnStart.Text = "▶ Başlat";
        btnStart.UseVisualStyleBackColor = true;
        btnStart.Click += btnStart_Click;
        //
        // btnStop
        //
        btnStop.Location = new Point(127, 11);
        btnStop.Name = "btnStop";
        btnStop.Size = new Size(110, 36);
        btnStop.TabIndex = 1;
        btnStop.TabStop = false;
        btnStop.Text = "■ Durdur";
        btnStop.UseVisualStyleBackColor = true;
        btnStop.Click += btnStop_Click;
        //
        // btnClear
        //
        btnClear.Location = new Point(243, 11);
        btnClear.Name = "btnClear";
        btnClear.Size = new Size(110, 36);
        btnClear.TabIndex = 2;
        btnClear.TabStop = false;
        btnClear.Text = "Temizle";
        btnClear.UseVisualStyleBackColor = true;
        btnClear.Click += btnClear_Click;
        //
        // btnSave
        //
        btnSave.Location = new Point(359, 11);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(150, 36);
        btnSave.TabIndex = 3;
        btnSave.TabStop = false;
        btnSave.Text = ".txt olarak kaydet";
        btnSave.UseVisualStyleBackColor = true;
        btnSave.Click += btnSave_Click;
        //
        // lblStatus
        //
        lblStatus.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblStatus.ForeColor = Color.Firebrick;
        lblStatus.Location = new Point(375, 11);
        lblStatus.Margin = new Padding(19, 3, 3, 3);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(220, 36);
        lblStatus.TabIndex = 3;
        lblStatus.Text = "● KAYIT KAPALI";
        lblStatus.TextAlign = ContentAlignment.MiddleLeft;
        //
        // lstLog
        //
        lstLog.Dock = DockStyle.Fill;
        lstLog.Font = new Font("Consolas", 10F);
        lstLog.IntegralHeight = false;
        lstLog.Location = new Point(0, 58);
        lstLog.Name = "lstLog";
        lstLog.SelectionMode = SelectionMode.None;
        lstLog.Size = new Size(760, 394);
        lstLog.TabIndex = 1;
        lstLog.TabStop = false;
        //
        // lblCount
        //
        lblCount.Dock = DockStyle.Bottom;
        lblCount.Location = new Point(0, 452);
        lblCount.Name = "lblCount";
        lblCount.Padding = new Padding(8, 0, 0, 0);
        lblCount.Size = new Size(760, 28);
        lblCount.TabIndex = 2;
        lblCount.Text = "Toplam kayıt: 0";
        lblCount.TextAlign = ContentAlignment.MiddleLeft;
        //
        // MainForm
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(760, 480);
        Controls.Add(lstLog);
        Controls.Add(lblCount);
        Controls.Add(pnlTop);
        KeyPreview = true;
        MinimumSize = new Size(560, 300);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Key Event Logger";
        KeyDown += MainForm_KeyDown;
        KeyUp += MainForm_KeyUp;
        KeyPress += MainForm_KeyPress;
        FormClosing += MainForm_FormClosing;
        pnlTop.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private FlowLayoutPanel pnlTop;
    private Button btnStart;
    private Button btnStop;
    private Button btnClear;
    private Button btnSave;
    private Label lblStatus;
    private ListBox lstLog;
    private Label lblCount;
}
