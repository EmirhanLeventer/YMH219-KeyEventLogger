namespace KeyEventLogger
{
    /// <summary>
    /// Yakalanan klavye olayının türü.
    /// </summary>
    public enum KeyEventKind
    {
        KeyDown,
        KeyUp,
        KeyPress
    }

    /// <summary>
    /// Tek bir klavye olayını temsil eder: ne zaman, hangi olay, hangi tuş.
    /// Nesne oluşturulduktan sonra değiştirilemez (özellikler yalnızca okunur).
    /// </summary>
    public class KeyEventRecord
    {
        public DateTime Time { get; }
        public KeyEventKind Kind { get; }
        public string KeyName { get; }

        public KeyEventRecord(DateTime time, KeyEventKind kind, string keyName)
        {
            if (string.IsNullOrWhiteSpace(keyName))
                throw new ArgumentException("Tuş adı boş olamaz.", nameof(keyName));

            Time = time;
            Kind = kind;
            KeyName = keyName;
        }

        /// <summary>
        /// Ekranda ve .txt dosyasında kullanılan ortak kayıt formatı.
        /// Örnek: 2026-11-05 14:32:10.123 | KeyDown  | A
        /// </summary>
        public override string ToString()
        {
            return $"{Time:yyyy-MM-dd HH:mm:ss.fff} | {Kind,-8} | {KeyName}";
        }
    }
}
