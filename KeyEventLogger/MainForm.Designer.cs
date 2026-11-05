namespace KeyEventLogger;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();

        pnlToolbar = new FlowLayoutPanel();
        btnStart = new Button();
        btnStop = new Button();
        btnClear = new Button();
        btnSave = new Button();
        btnStats = new Button();
        lblStatus = new Label();

        pnlFilter = new FlowLayoutPanel();
        lblRange = new Label();
        dtpStart = new DateTimePicker();
        lblDash = new Label();
        dtpEnd = new DateTimePicker();
        btnFilter = new Button();
        btnShowAll = new Button();
        lblFilter = new Label();

        lstLog = new ListBox();
        lblCount = new Label();

        pnlToolbar.SuspendLayout();
        pnlFilter.SuspendLayout();
        SuspendLayout();
        //
        // pnlToolbar
        //
        pnlToolbar.Controls.Add(btnStart);
        pnlToolbar.Controls.Add(btnStop);
        pnlToolbar.Controls.Add(btnClear);
        pnlToolbar.Controls.Add(btnSave);
        pnlToolbar.Controls.Add(btnStats);
        pnlToolbar.Controls.Add(lblStatus);
        pnlToolbar.Dock = DockStyle.Top;
        pnlToolbar.Name = "pnlToolbar";
        pnlToolbar.Padding = new Padding(10, 10, 10, 8);
        pnlToolbar.Size = new Size(860, 56);
        pnlToolbar.TabIndex = 0;
        pnlToolbar.WrapContents = false;
        //
        // btnStart
        //
        btnStart.Margin = new Padding(0, 0, 6, 0);
        btnStart.Name = "btnStart";
        btnStart.Size = new Size(110, 34);
        btnStart.TabStop = false;
        btnStart.Text = "▶  Başlat";
        btnStart.Click += btnStart_Click;
        //
        // btnStop
        //
        btnStop.Margin = new Padding(0, 0, 6, 0);
        btnStop.Name = "btnStop";
        btnStop.Size = new Size(110, 34);
        btnStop.TabStop = false;
        btnStop.Text = "■  Durdur";
        btnStop.Click += btnStop_Click;
        //
        // btnClear
        //
        btnClear.Margin = new Padding(0, 0, 6, 0);
        btnClear.Name = "btnClear";
        btnClear.Size = new Size(100, 34);
        btnClear.TabStop = false;
        btnClear.Text = "Temizle";
        btnClear.Click += btnClear_Click;
        //
        // btnSave
        //
        btnSave.Margin = new Padding(0, 0, 6, 0);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(150, 34);
        btnSave.TabStop = false;
        btnSave.Text = ".txt olarak kaydet";
        btnSave.Click += btnSave_Click;
        //
        // btnStats
        //
        btnStats.Margin = new Padding(0, 0, 6, 0);
        btnStats.Name = "btnStats";
        btnStats.Size = new Size(110, 34);
        btnStats.TabStop = false;
        btnStats.Text = "İstatistik";
        btnStats.Click += btnStats_Click;
        //
        // lblStatus
        //
        lblStatus.AutoSize = true;
        lblStatus.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblStatus.ForeColor = Color.Firebrick;
        lblStatus.Margin = new Padding(20, 7, 3, 0);
        lblStatus.Name = "lblStatus";
        lblStatus.Text = "● KAYIT KAPALI";
        //
        // pnlFilter
        //
        pnlFilter.Controls.Add(lblRange);
        pnlFilter.Controls.Add(dtpStart);
        pnlFilter.Controls.Add(lblDash);
        pnlFilter.Controls.Add(dtpEnd);
        pnlFilter.Controls.Add(btnFilter);
        pnlFilter.Controls.Add(btnShowAll);
        pnlFilter.Controls.Add(lblFilter);
        pnlFilter.Dock = DockStyle.Top;
        pnlFilter.Location = new Point(0, 56);
        pnlFilter.Name = "pnlFilter";
        pnlFilter.Padding = new Padding(10, 9, 10, 9);
        pnlFilter.Size = new Size(860, 48);
        pnlFilter.TabIndex = 1;
        pnlFilter.WrapContents = false;
        //
        // lblRange
        //
        lblRange.AutoSize = true;
        lblRange.Margin = new Padding(0, 8, 8, 0);
        lblRange.Name = "lblRange";
        lblRange.Text = "Aralık:";
        //
        // dtpStart
        //
        dtpStart.CustomFormat = "dd.MM.yyyy HH:mm:ss";
        dtpStart.Format = DateTimePickerFormat.Custom;
        dtpStart.Margin = new Padding(0, 4, 4, 0);
        dtpStart.Name = "dtpStart";
        dtpStart.ShowUpDown = true;
        dtpStart.Size = new Size(150, 23);
        //
        // lblDash
        //
        lblDash.AutoSize = true;
        lblDash.Margin = new Padding(0, 8, 4, 0);
        lblDash.Name = "lblDash";
        lblDash.Text = "—";
        //
        // dtpEnd
        //
        dtpEnd.CustomFormat = "dd.MM.yyyy HH:mm:ss";
        dtpEnd.Format = DateTimePickerFormat.Custom;
        dtpEnd.Margin = new Padding(0, 4, 8, 0);
        dtpEnd.Name = "dtpEnd";
        dtpEnd.ShowUpDown = true;
        dtpEnd.Size = new Size(150, 23);
        //
        // btnFilter
        //
        btnFilter.Margin = new Padding(0, 1, 4, 0);
        btnFilter.Name = "btnFilter";
        btnFilter.Size = new Size(90, 28);
        btnFilter.TabStop = false;
        btnFilter.Text = "Filtrele";
        btnFilter.Click += btnFilter_Click;
        //
        // btnShowAll
        //
        btnShowAll.Margin = new Padding(0, 1, 8, 0);
        btnShowAll.Name = "btnShowAll";
        btnShowAll.Size = new Size(120, 28);
        btnShowAll.TabStop = false;
        btnShowAll.Text = "Tümünü Göster";
        btnShowAll.Click += btnShowAll_Click;
        //
        // lblFilter
        //
        lblFilter.AutoSize = true;
        lblFilter.ForeColor = Color.Gray;
        lblFilter.Margin = new Padding(8, 8, 0, 0);
        lblFilter.Name = "lblFilter";
        lblFilter.Text = "Filtre: yok";
        //
        // lstLog
        //
        lstLog.BorderStyle = BorderStyle.None;
        lstLog.Dock = DockStyle.Fill;
        lstLog.Font = new Font("Consolas", 10F);
        lstLog.IntegralHeight = false;
        lstLog.ItemHeight = 17;
        lstLog.Name = "lstLog";
        lstLog.SelectionMode = SelectionMode.None;
        lstLog.TabIndex = 2;
        lstLog.TabStop = false;
        //
        // lblCount
        //
        lblCount.Dock = DockStyle.Bottom;
        lblCount.Font = new Font("Segoe UI", 9F);
        lblCount.Location = new Point(0, 530);
        lblCount.Name = "lblCount";
        lblCount.Padding = new Padding(12, 0, 0, 0);
        lblCount.Size = new Size(860, 30);
        lblCount.TabIndex = 3;
        lblCount.Text = "Toplam kayıt: 0";
        lblCount.TextAlign = ContentAlignment.MiddleLeft;
        //
        // MainForm
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(860, 560);
        Controls.Add(lstLog);
        Controls.Add(lblCount);
        Controls.Add(pnlFilter);
        Controls.Add(pnlToolbar);
        Font = new Font("Segoe UI", 9.75F);
        KeyPreview = true;
        MinimumSize = new Size(680, 420);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Key Event Logger";
        KeyDown += MainForm_KeyDown;
        KeyUp += MainForm_KeyUp;
        KeyPress += MainForm_KeyPress;
        FormClosing += MainForm_FormClosing;
        pnlToolbar.ResumeLayout(false);
        pnlToolbar.PerformLayout();
        pnlFilter.ResumeLayout(false);
        pnlFilter.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private FlowLayoutPanel pnlToolbar;
    private Button btnStart;
    private Button btnStop;
    private Button btnClear;
    private Button btnSave;
    private Button btnStats;
    private Label lblStatus;

    private FlowLayoutPanel pnlFilter;
    private Label lblRange;
    private DateTimePicker dtpStart;
    private Label lblDash;
    private DateTimePicker dtpEnd;
    private Button btnFilter;
    private Button btnShowAll;
    private Label lblFilter;

    private ListBox lstLog;
    private Label lblCount;
}
