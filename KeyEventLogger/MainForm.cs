namespace KeyEventLogger;

/// <summary>
/// Ana pencere. Yalnızca arayüzden sorumludur:
/// butonları KeyLogSession'a yönlendirir, oturumdan gelen olaylarla ekranı günceller.
/// </summary>
public partial class MainForm : Form
{
    private readonly KeyLogSession _session = new KeyLogSession();

    // KeyPress ile öğrenilen gerçek karakteri, en son eklenen KeyDown satırına yansıtmak için.
    private Keys _lastDownCode = Keys.None;
    private KeyEventRecord? _pendingDownRecord;
    private int _pendingDownIndex = -1;

    // Her fiziksel tuşun ürettiği gerçek karakter (Türkçe harfler, noktalama vb.).
    private readonly Dictionary<Keys, char> _charByCode = new Dictionary<Keys, char>();

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

    // ---- Buton olayları: işi oturuma devreder ----

    private void btnStart_Click(object? sender, EventArgs e)
    {
        _session.Start();
        FocusLogList();
    }

    private void btnStop_Click(object? sender, EventArgs e)
    {
        _session.Stop();
    }

    private void btnClear_Click(object? sender, EventArgs e)
    {
        _session.Clear();
        if (_session.IsRecording)
            FocusLogList();
    }

    /// <summary>
    /// Kayıt sırasında odağı, buton olmayan ListBox'ta tutar. Böylece:
    /// 1) Tuşlar butonları yanlışlıkla tetiklemez (Space/Enter),
    /// 2) KeyPress olayı serbestçe çalışır ve gerçek karakteri verir,
    /// 3) ilk KeyDown bir butona takılıp kaybolmaz.
    /// ListBox'ın SelectionMode'u None olduğu için harfler listede gezinmeye yol açmaz.
    /// </summary>
    private void FocusLogList()
    {
        lstLog.Focus();
    }

    // ---- Klavye olayları ----
    // KeyPreview = true olduğu için, odak hangi kontrolde olursa olsun tuş olayları önce Form'a gelir.
    // Bu olaylar yalnızca BU pencere aktifken oluşur; başka uygulamalardaki tuşlar yakalanmaz.

    private void MainForm_KeyDown(object? sender, KeyEventArgs e)
    {
        _lastDownCode = e.KeyCode;
        string name = NameFor(e.KeyCode, e.Control, e.Alt, e.Shift, CachedChar(e.KeyCode));
        _session.Add(KeyEventKind.KeyDown, name);
        // Not: SuppressKeyPress KULLANMIYORUZ; kullansaydık KeyPress olayı da engellenir
        // ve gerçek karakteri (ör. Türkçe 'ş') okuyamazdık. Butonların tetiklenmesini
        // odağı ListBox'ta tutarak (FocusLogList) engelliyoruz.
    }

    private void MainForm_KeyUp(object? sender, KeyEventArgs e)
    {
        string name = NameFor(e.KeyCode, e.Control, e.Alt, e.Shift, CachedChar(e.KeyCode));
        _session.Add(KeyEventKind.KeyUp, name);
    }

    /// <summary>
    /// KeyPress yalnızca karakter üreten tuşlarda oluşur ve klavye düzenini (Türkçe Q vb.)
    /// dikkate alarak gerçek karakteri verir. Bu karakteri hem önbelleğe alırız hem de
    /// hemen önce eklenen KeyDown satırının adını düzeltmek için kullanırız.
    /// </summary>
    private void MainForm_KeyPress(object? sender, KeyPressEventArgs e)
    {
        char c = e.KeyChar;
        if (char.IsControl(c))
            return; // Enter, Tab, Ctrl+C gibi tuşların KeyPress karakteri okunmaz; dostane adı kullanılır.

        _charByCode[_lastDownCode] = c;

        if (_pendingDownRecord != null &&
            _pendingDownIndex >= 0 && _pendingDownIndex < lstLog.Items.Count)
        {
            _pendingDownRecord.SetKeyName(DisplayChar(c));
            lstLog.Items[_pendingDownIndex] = _pendingDownRecord; // satırı yeniden çizdir
            _pendingDownRecord = null;
        }
    }

    // ---- Oturum olayları: ekranı günceller ----

    private void Session_RecordAdded(object? sender, KeyEventRecord record)
    {
        int index = lstLog.Items.Add(record);        // ListBox, record.ToString() metnini gösterir
        lstLog.TopIndex = lstLog.Items.Count - 1;     // en son kayda kaydır

        // Yalnızca KeyDown kayıtlarını, KeyPress ile düzeltilebilsin diye hatırla.
        if (record.Kind == KeyEventKind.KeyDown)
        {
            _pendingDownRecord = record;
            _pendingDownIndex = index;
        }

        UpdateUi();
    }

    private void Session_RecordingStateChanged(object? sender, EventArgs e)
    {
        UpdateUi();
    }

    private void Session_RecordsCleared(object? sender, EventArgs e)
    {
        lstLog.Items.Clear();
        _pendingDownRecord = null;
        _pendingDownIndex = -1;
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

    // ---- Yardımcılar ----

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

    private char? CachedChar(Keys code)
    {
        return _charByCode.TryGetValue(code, out char c) ? c : (char?)null;
    }

    private const uint MAPVK_VK_TO_CHAR = 0x02;

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern uint MapVirtualKey(uint uCode, uint uMapType);

    /// <summary>
    /// Fiziksel tuşun, geçerli klavye düzenindeki (ör. Türkçe Q) temel karakterini döndürür.
    /// Bu YALNIZCA bir düzen çevirisidir: tuş kodunu karaktere çevirir. Klavye dinlemez,
    /// hook kurmaz, başka uygulamadan veri almaz, arka planda çalışmaz, tuş durumu okumaz.
    /// Yani föydeki gözetim yasaklarının hiçbirine girmez. Yalnızca KeyPress'ten karakter
    /// alınamayan durumlar için yedektir (ör. Ctrl'lü tuşun ilk basışında önbellek boşken).
    /// </summary>
    private static char? LayoutChar(Keys code)
    {
        uint r = MapVirtualKey((uint)code, MAPVK_VK_TO_CHAR);
        if (r == 0) return null;
        if ((r & 0x80000000) != 0) return null; // ölü tuş (dead key)
        char c = (char)(r & 0xFFFF);
        if (c == 0 || char.IsControl(c)) return null;
        return c;
    }

    /// <summary>
    /// Tuşu okunur bir ada çevirir. Öncelik sırası:
    /// 1) Değiştirici tuşun kendisi → "Ctrl"/"Shift"/"Alt"
    /// 2) Ctrl/Alt yoksa ve gerçek karakter biliniyorsa → o karakter (ör. "ş", "A", "@")
    /// 3) Aksi halde değiştirici öneki + dostane tuş adı (ör. "Ctrl+C", "Shift+Tab", "F5")
    /// </summary>
    private string NameFor(Keys code, bool ctrl, bool alt, bool shift, char? resolved)
    {
        // 1) Değiştirici tuşun kendisine basıldıysa yalnızca adını yaz.
        if (code is Keys.ControlKey or Keys.LControlKey or Keys.RControlKey) return "Ctrl";
        if (code is Keys.ShiftKey or Keys.LShiftKey or Keys.RShiftKey) return "Shift";
        if (code is Keys.Menu or Keys.LMenu or Keys.RMenu) return "Alt";

        // 2) Ctrl/Alt yoksa tek karakter göster.
        if (!ctrl && !alt)
        {
            // Harfler: büyük/küçük durumunu o anki Shift + CapsLock'tan KESİN hesapla.
            // (Önbelleğe güvenmeyiz; böylece ilk basışta doğru olur, titreme olmaz.)
            if (code >= Keys.A && code <= Keys.Z)
            {
                bool upper = shift ^ Control.IsKeyLocked(Keys.CapsLock);
                return ((char)((upper ? 'A' : 'a') + (code - Keys.A))).ToString();
            }
            // Türkçe/OEM/sembol/boşluk: KeyPress'ten öğrenilen gerçek karakter.
            if (resolved is char rc)
                return DisplayChar(rc);
            // İlk basış ve önbellek boşsa: klavye düzeninden çevir (ör. Türkçe 'ş').
            if (LayoutChar(code) is char lc)
                return DisplayChar(lc);
            // Karakteri hiç çözülemeyen tuşlar (ok, F-tuşları vb.): dostane ad.
            return FriendlyName(code);
        }

        // 3) Ctrl/Alt varsa "önek + tuş" biçimi.
        var parts = new List<string>();
        if (ctrl) parts.Add("Ctrl");
        if (alt) parts.Add("Alt");
        if (shift) parts.Add("Shift");

        string keyPart;
        if (code >= Keys.A && code <= Keys.Z)
            keyPart = ((char)('A' + (code - Keys.A))).ToString();  // Ctrl+C (geleneksel büyük harf)
        else if (resolved is char rc2)
            keyPart = DisplayChar(rc2);                            // Ctrl+ş (önbellekten)
        else if (LayoutChar(code) is char lc2)
            keyPart = DisplayChar(lc2);                            // Ctrl+ş (ilk basışta düzen çevirisiyle)
        else
            keyPart = FriendlyName(code);                          // Ctrl+F5, Ctrl+Space vb.
        parts.Add(keyPart);

        return string.Join("+", parts);
    }

    private static string DisplayChar(char c)
    {
        return c == ' ' ? "Space" : c.ToString();
    }

    /// <summary>
    /// Karakter üretmeyen/özel tuşlar için okunur ad. Türkçe/OEM harfler için bu metot yerine
    /// KeyPress'ten gelen gerçek karakter kullanılır.
    /// </summary>
    private static string FriendlyName(Keys code)
    {
        switch (code)
        {
            case Keys.Space: return "Space";
            case Keys.Return: return "Enter";
            case Keys.Escape: return "Esc";
            case Keys.Back: return "Backspace";
            case Keys.Tab: return "Tab";
            case Keys.Delete: return "Delete";
            case Keys.Insert: return "Insert";
            case Keys.Home: return "Home";
            case Keys.End: return "End";
            case Keys.PageUp: return "PageUp";
            case Keys.PageDown: return "PageDown";
            case Keys.Left: return "←";
            case Keys.Right: return "→";
            case Keys.Up: return "↑";
            case Keys.Down: return "↓";
            case Keys.CapsLock: return "CapsLock";
        }

        if (code >= Keys.A && code <= Keys.Z)
            return code.ToString();
        if (code >= Keys.D0 && code <= Keys.D9)
            return ((char)('0' + (code - Keys.D0))).ToString();
        if (code >= Keys.NumPad0 && code <= Keys.NumPad9)
            return "Num" + (code - Keys.NumPad0);
        if (code >= Keys.F1 && code <= Keys.F24)
            return code.ToString();

        return code.ToString(); // Son çare: ham ad (çok kısa süre görünür, KeyPress düzeltir).
    }
}
