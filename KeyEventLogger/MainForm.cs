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

        AttachPreviewKeyDown(this);
        UpdateUi();
    }

    // ---- Klavye olayları ----
    // KeyPreview = true olduğu için, odak hangi kontrolde olursa olsun tuş olayları önce Form'a gelir.
    // Bu olaylar yalnızca BU pencere aktifken oluşur; başka uygulamalardaki tuşlar yakalanmaz.

    private void MainForm_KeyDown(object? sender, KeyEventArgs e)
    {
        if (_session.Add(KeyEventKind.KeyDown, DescribeKey(e)))
            SuppressUnlessAlt(e);
    }

    private void MainForm_KeyUp(object? sender, KeyEventArgs e)
    {
        if (_session.Add(KeyEventKind.KeyUp, DescribeKey(e)))
            SuppressUnlessAlt(e);
    }

    /// <summary>
    /// Kayıt açıkken tuşun butonlara ulaşmasını engeller (Space/Enter ile yanlışlıkla tıklanmasın).
    /// Alt'lı tuşlara dokunmaz; böylece Alt+F4 ile pencere her zaman kapatılabilir.
    /// </summary>
    private static void SuppressUnlessAlt(KeyEventArgs e)
    {
        if (!e.Alt)
            e.SuppressKeyPress = true;
    }

    /// <summary>
    /// Tab ve ok tuşları normalde "diyalog tuşu"dur: odağı değiştirir ve KeyDown olayı oluşmaz.
    /// Kayıt açıkken bunları normal tuş saydırarak KeyDown'a ulaşmalarını sağlar.
    /// Form'daki tüm kontrollere (iç içe olanlar dahil) özyinelemeli olarak bağlanır.
    /// </summary>
    private void AttachPreviewKeyDown(Control parent)
    {
        foreach (Control child in parent.Controls)
        {
            child.PreviewKeyDown += (sender, e) =>
            {
                if (_session.IsRecording)
                    e.IsInputKey = true;
            };
            AttachPreviewKeyDown(child);
        }
    }

    /// <summary>
    /// Tuşu okunur bir ada çevirir. Örnekler: "A", "Ctrl+C", "Shift+D1", "Shift".
    /// </summary>
    private static string DescribeKey(KeyEventArgs e)
    {
        // Değiştirici tuşun kendisine basıldıysa yalnızca adını yaz.
        switch (e.KeyCode)
        {
            case Keys.ControlKey: return "Ctrl";
            case Keys.ShiftKey: return "Shift";
            case Keys.Menu: return "Alt";
        }

        var parts = new List<string>();
        if (e.Control) parts.Add("Ctrl");
        if (e.Alt) parts.Add("Alt");
        if (e.Shift) parts.Add("Shift");
        parts.Add(e.KeyCode.ToString());

        return string.Join("+", parts);
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
