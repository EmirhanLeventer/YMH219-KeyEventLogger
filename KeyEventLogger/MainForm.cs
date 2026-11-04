namespace KeyEventLogger;

/// <summary>
/// Ana pencere. Yalnızca arayüzden sorumludur:
/// butonları KeyLogSession'a yönlendirir, oturumdan gelen olaylarla ekranı günceller.
/// </summary>
public partial class MainForm : Form
{
    private readonly KeyLogSession _session = new KeyLogSession();

    public MainForm()
    {
        InitializeComponent();

        // KeyLogSession'ın olaylarına abone ol (+=).
        _session.RecordAdded += Session_RecordAdded;
        _session.RecordingStateChanged += Session_RecordingStateChanged;
        _session.RecordsCleared += Session_RecordsCleared;

        UpdateUi();
    }

    // ---- Buton olayları: işi oturuma devreder ----

    private void btnStart_Click(object? sender, EventArgs e)
    {
        _session.Start();
    }

    private void btnStop_Click(object? sender, EventArgs e)
    {
        _session.Stop();
    }

    private void btnClear_Click(object? sender, EventArgs e)
    {
        _session.Clear();
    }

    // ---- Oturum olayları: ekranı günceller ----

    private void Session_RecordAdded(object? sender, KeyEventRecord record)
    {
        lstLog.Items.Add(record);                    // ListBox, record.ToString() metnini gösterir
        lstLog.TopIndex = lstLog.Items.Count - 1;    // en son kayda kaydır
        UpdateUi();
    }

    private void Session_RecordingStateChanged(object? sender, EventArgs e)
    {
        UpdateUi();
    }

    private void Session_RecordsCleared(object? sender, EventArgs e)
    {
        lstLog.Items.Clear();
        UpdateUi();
    }

    /// <summary>
    /// Butonların aktifliğini, durum göstergesini ve sayacı oturumun güncel durumuna göre ayarlar.
    /// </summary>
    private void UpdateUi()
    {
        bool recording = _session.IsRecording;

        btnStart.Enabled = !recording;
        btnStop.Enabled = recording;
        btnClear.Enabled = _session.Records.Count > 0;

        lblStatus.Text = recording ? "● KAYIT AÇIK" : "● KAYIT KAPALI";
        lblStatus.ForeColor = recording ? Color.ForestGreen : Color.Firebrick;
        Text = recording ? "Key Event Logger — KAYIT AÇIK" : "Key Event Logger";

        lblCount.Text = $"Toplam kayıt: {_session.Records.Count}";
    }
}
