namespace KeyEventLogger
{
    /// <summary>
    /// Kayıt oturumunu yönetir: kaydın açık/kapalı durumu ve yakalanan olayların listesi.
    /// Arayüzden bağımsızdır; değişiklikleri event'ler ile dışarıya bildirir.
    /// </summary>
    public class KeyLogSession
    {
        // Liste yalnızca bu sınıfın içinden değiştirilebilir.
        private readonly List<KeyEventRecord> _records = new List<KeyEventRecord>();

        /// <summary>Kayıtların salt okunur görünümü (dışarıdan Add/Remove yapılamaz).</summary>
        public IReadOnlyList<KeyEventRecord> Records => _records;

        /// <summary>Kayıt şu anda açık mı? Dışarıdan yalnızca okunabilir.</summary>
        public bool IsRecording { get; private set; }

        /// <summary>Yeni bir kayıt eklendiğinde tetiklenir.</summary>
        public event EventHandler<KeyEventRecord>? RecordAdded;

        /// <summary>Kayıt başlatıldığında veya durdurulduğunda tetiklenir.</summary>
        public event EventHandler? RecordingStateChanged;

        /// <summary>Tüm kayıtlar temizlendiğinde tetiklenir.</summary>
        public event EventHandler? RecordsCleared;

        public void Start()
        {
            if (IsRecording)
                return; // Zaten açıksa tekrar bildirim yapma.

            IsRecording = true;
            RecordingStateChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Stop()
        {
            if (!IsRecording)
                return;

            IsRecording = false;
            RecordingStateChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Clear()
        {
            _records.Clear();
            RecordsCleared?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Kayıt açıksa yeni bir olay ekler. Kayıt kapalıysa hiçbir şey yapmaz.
        /// </summary>
        /// <returns>Kayıt eklendiyse true.</returns>
        public bool Add(KeyEventKind kind, string keyName)
        {
            if (!IsRecording)
                return false;

            var record = new KeyEventRecord(DateTime.Now, kind, keyName);
            _records.Add(record);
            RecordAdded?.Invoke(this, record);
            return true;
        }
    }
}
