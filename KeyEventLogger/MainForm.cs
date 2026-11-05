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

    // Her fiziksel tuşun ürettiği gerçek karakter (KeyPress'ten; shift'li hâli de dahil).
    private readonly Dictionary<Keys, char> _charByCode = new Dictionary<Keys, char>();

    // Klavye düzeninin DEĞİŞMEZ temel (shift'siz, küçük) karakter haritası; açılışta bir kez doldurulur.
    private readonly Dictionary<Keys, char> _baseCharByCode = new Dictionary<Keys, char>();

    // Türkçe büyük/küçük dönüşümü için (ı↔I, i↔İ).
    private static readonly System.Globalization.CultureInfo Tr =
        System.Globalization.CultureInfo.GetCultureInfo("tr-TR");

    // Son kaydetmeden sonra yeni kayıt eklendi mi? (çıkışta uyarı için)
    private bool _dirty = false;

    public MainForm()
    {
        InitializeComponent();

        // KeyLogSession'ın olaylarına abone ol (+=).
        _session.RecordAdded += Session_RecordAdded;
        _session.RecordingStateChanged += Session_RecordingStateChanged;
        _session.RecordsCleared += Session_RecordsCleared;

        PrewarmLayoutChars();
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
        // Yanlışlıkla silmeyi önlemek için onay iste.
        if (_session.Records.Count > 0)
        {
            var result = MessageBox.Show("Ekrandaki tüm kayıtlar silinsin mi?", "Temizle",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No)
            {
                if (_session.IsRecording)
                    FocusLogList();
                return;
            }
        }

        _session.Clear();
        if (_session.IsRecording)
            FocusLogList();
    }

    private void btnSave_Click(object? sender, EventArgs e)
    {
        if (_session.Records.Count == 0)
        {
            MessageBox.Show("Kaydedilecek kayıt yok.", "Bilgi",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        // Kullanıcı dosya adını ve yerini kendisi seçer (föy: "kullanıcı isterse kaydetsin").
        using var dialog = new SaveFileDialog
        {
            Title = "Kayıtları kaydet",
            Filter = "Metin dosyası (*.txt)|*.txt",
            FileName = $"keylog_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
        };

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            // Dosya işlemleri hata verebilir (izin yok, disk dolu vb.); yakalayıp bildiriyoruz.
            try
            {
                LogFileWriter.Save(dialog.FileName, _session.Records);
                _dirty = false; // başarıyla kaydedildi
                MessageBox.Show($"{_session.Records.Count} kayıt kaydedildi:\n{dialog.FileName}",
                    "Kaydedildi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Dosya kaydedilemedi:\n{ex.Message}", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        if (_session.IsRecording)
            FocusLogList();
    }

    private void btnStats_Click(object? sender, EventArgs e)
    {
        var stats = new KeyStatistics(_session.Records, 5);

        if (stats.TotalKeyDown == 0)
        {
            MessageBox.Show("Henüz istatistik için yeterli kayıt yok.", "İstatistik",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        string topList = string.Join("\n",
            stats.TopKeys.Select((kv, i) => $"{i + 1}. {kv.Key}  —  {kv.Value} kez"));

        string message =
            $"Toplam tuş basışı (KeyDown): {stats.TotalKeyDown}\n\n" +
            $"En sık kullanılan tuşlar:\n{topList}";

        MessageBox.Show(message, "İstatistik", MessageBoxButtons.OK, MessageBoxIcon.Information);

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
        if (!_session.IsRecording)
            return;

        string name = NameFor(e.KeyCode, e.Control, e.Alt, e.Shift, CachedChar(e.KeyCode));
        _session.Add(KeyEventKind.KeyDown, name);

        // Kayıt açıkken TÜM tuşların yan etkisini engelle: ListBox kaydırma, ok ile odak değişimi,
        // Space/Enter ile buton tıklama ve Alt+F4 ile kapanma. Böylece Alt+F4 de yalnızca log'a düşer.
        // Handled kullanıyoruz; SuppressKeyPress DEĞİL -> KeyPress yine çalışıp karakteri verir.
        // (Kapatmak için: önce Durdur, ya da başlık çubuğundaki X.)
        e.Handled = true;
    }

    private void MainForm_KeyUp(object? sender, KeyEventArgs e)
    {
        if (!_session.IsRecording)
            return;

        string name = NameFor(e.KeyCode, e.Control, e.Alt, e.Shift, CachedChar(e.KeyCode));
        _session.Add(KeyEventKind.KeyUp, name);

        e.Handled = true;
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

        _dirty = true; // kaydedilmemiş yeni kayıt var
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
        _dirty = false; // silindi, kaybedilecek kayıt yok
        UpdateUi();
    }

    private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        // Kaydedilmemiş kayıt varken kapatmaya çalışırsa uyar (veri kaybını önle).
        if (_dirty && _session.Records.Count > 0)
        {
            var result = MessageBox.Show(
                "Kaydedilmemiş kayıtlar var. Yine de çıkmak istiyor musunuz?",
                "Çıkış", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.No)
                e.Cancel = true; // kapanmayı iptal et
        }
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
        btnSave.Enabled = _session.Records.Count > 0;
        btnStats.Enabled = _session.Records.Count > 0;

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

    private const uint MAPVK_VK_TO_VSC = 0x00;

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern IntPtr GetKeyboardLayout(uint idThread);

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern uint MapVirtualKeyEx(uint uCode, uint uMapType, IntPtr dwhkl);

    [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
    private static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[] lpKeyState,
        System.Text.StringBuilder pwszBuff, int cchBuff, uint wFlags, IntPtr dwhkl);

    /// <summary>
    /// Program açılışında bir kez çalışır: klavye düzenindeki (ör. Türkçe Q) her tuşun
    /// değiştiricisiz (shift'siz) temel karakterini hesaplayıp önbelleğe (_charByCode) koyar.
    /// Böylece bir tuşa ilk kez basıldığında (özellikle Ctrl'lü, KeyPress'in karakter vermediği
    /// durumda) bile doğru karakter gösterilebilir.
    ///
    /// GÜVENLİK NOTU: Bu bir düzen çevirisidir; tuş kodunu karaktere çevirir. Klavye dinlemez,
    /// global hook kurmaz, başka uygulamadan veri almaz, arka planda çalışmaz. Üstelik yalnızca
    /// açılışta bir kez çağrılır. Föydeki gözetim yasaklarının hiçbirine girmez.
    /// </summary>
    private void PrewarmLayoutChars()
    {
        IntPtr hkl = GetKeyboardLayout(0);
        byte[] keyState = new byte[256]; // tüm değiştiriciler kapalı → temel karakter
        var sb = new System.Text.StringBuilder(8);

        for (uint vk = 0x20; vk <= 0xFF; vk++)
        {
            uint scan = MapVirtualKeyEx(vk, MAPVK_VK_TO_VSC, hkl);
            if (scan == 0)
                continue;

            sb.Clear();
            int rc = ToUnicodeEx(vk, scan, keyState, sb, sb.Capacity, 0, hkl);
            if (rc < 0)
            {
                // Ölü tuş (ör. şapka): kernel durumunu bozmamak için ikinci çağrıyla sıfırla, atla.
                sb.Clear();
                ToUnicodeEx(vk, scan, keyState, sb, sb.Capacity, 0, hkl);
                continue;
            }
            if (rc > 0 && sb.Length > 0 && !char.IsControl(sb[0]))
                _baseCharByCode[(Keys)vk] = sb[0];
        }
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

        // Tuşun klavye düzenindeki temel (shift'siz, küçük) karakteri. Harf mi, onu buradan anlarız.
        char? baseC = _baseCharByCode.TryGetValue(code, out char bc) ? bc : (char?)null;
        bool isLetter = baseC is char bl && char.IsLetter(bl);

        // 2) Ctrl/Alt yoksa tek karakter göster.
        if (!ctrl && !alt)
        {
            // Harfler (Türkçe dahil): büyük/küçük durumunu o anki Shift + CapsLock'tan KESİN hesapla.
            // Temel harfi klavye düzeninden alırız (VK_I Türkçe Q'da 'ı' üretir, 'i' değil), böylece
            // KeyDown ve KeyUp AYNI harfi yazar ve ilk basışta titreme/yanlış büyük-küçük olmaz.
            if (isLetter)
            {
                bool upper = shift ^ Control.IsKeyLocked(Keys.CapsLock);
                char b = baseC!.Value;
                return (upper ? char.ToUpper(b, Tr) : b).ToString();
            }
            // Harf olmayan (rakam/sembol): shift farklı sembol üretebilir -> KeyPress'ten gelen gerçek karakter.
            if (resolved is char rc)
                return DisplayChar(rc);
            if (baseC is char b2)
                return DisplayChar(b2);
            // Karakteri olmayan tuşlar (ok, F-tuşları vb.): dostane ad.
            return FriendlyName(code);
        }

        // 3) Ctrl/Alt varsa "önek + tuş" biçimi.
        var parts = new List<string>();
        if (ctrl) parts.Add("Ctrl");
        if (alt) parts.Add("Alt");
        if (shift) parts.Add("Shift");

        string keyPart;
        if (isLetter)
            keyPart = char.ToUpper(baseC!.Value, Tr).ToString();  // Ctrl+C, Ctrl+Ş (geleneksel büyük harf)
        else if (resolved is char rc2)
            keyPart = DisplayChar(rc2);                            // Ctrl+1 vb.
        else if (baseC is char b3)
            keyPart = DisplayChar(b3);                             // Ctrl+ş (ilk basışta düzen haritasından)
        else
            keyPart = FriendlyName(code);                         // Ctrl+F5, Ctrl+Space vb.
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
