namespace KeyEventLogger;

/// <summary>
/// Kayıtlardan basit istatistik çıkarır: toplam tuş basışı ve en sık kullanılan tuşlar.
/// Yalnızca KeyDown olaylarını sayar (her fiziksel basış bir kez sayılsın diye).
/// </summary>
public class KeyStatistics
{
    public int TotalKeyDown { get; }

    /// <summary>En sık kullanılan tuşlar: (tuş adı, kaç kez) çiftleri, çoktan aza sıralı.</summary>
    public IReadOnlyList<KeyValuePair<string, int>> TopKeys { get; }

    public KeyStatistics(IEnumerable<KeyEventRecord> records, int topCount = 5)
    {
        var downs = records.Where(r => r.Kind == KeyEventKind.KeyDown).ToList();

        TotalKeyDown = downs.Count;

        TopKeys = downs
            .GroupBy(r => r.KeyName)                                   // aynı tuşları grupla
            .Select(g => new KeyValuePair<string, int>(g.Key, g.Count())) // (tuş, adet)
            .OrderByDescending(p => p.Value)                          // çoktan aza
            .ThenBy(p => p.Key)                                       // eşitlikte alfabetik
            .Take(topCount)
            .ToList();
    }
}
