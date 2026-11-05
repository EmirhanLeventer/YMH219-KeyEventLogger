using System.Text;

namespace KeyEventLogger;

/// <summary>
/// Kayıtları bir .txt dosyasına yazmaktan sorumludur.
/// Dosya işlemleri burada toplanır; Form yalnızca "şu kayıtları şu yola yaz" der.
/// </summary>
public static class LogFileWriter
{
    /// <summary>
    /// Verilen kayıtları UTF-8 kodlamasıyla belirtilen dosyaya yazar (varsa üzerine yazar).
    /// UTF-8, Türkçe karakterlerin (ş, İ, ğ ...) dosyada doğru görünmesini sağlar.
    /// Hata olursa çağıran taraf yakalasın diye burada try/catch yoktur.
    /// </summary>
    public static void Save(string path, IEnumerable<KeyEventRecord> records)
    {
        using var writer = new StreamWriter(path, append: false, Encoding.UTF8);

        writer.WriteLine($"# Key Event Logger kaydı — {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        writer.WriteLine("# Zaman | Olay | Tuş");

        foreach (var record in records)
            writer.WriteLine(record.ToString());
    }
}
