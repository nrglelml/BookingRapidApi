namespace HotelSearch.Models;

public record FacilityItem(string Name, string Icon);

// Booking'den İngilizce gelen olanak isimlerini Türkçe isim + Bootstrap ikonuna çevirir.
public static class FacilityTranslator
{
    private const string DefaultIcon = "bi-check2-circle";


    private static readonly (string Keyword, string Name, string Icon)[] Rules =
    {
        ("free wifi", "Ücretsiz Wi-Fi", "bi-wifi"),
        ("wifi", "Wi-Fi", "bi-wifi"),
        ("airport shuttle", "Havalimanı servisi", "bi-airplane"),
        ("shuttle", "Servis aracı", "bi-bus-front"),
        ("non-smoking", "Sigara içilmeyen odalar", "bi-x-circle"),
        ("family rooms", "Aile odaları", "bi-people"),
        ("room service", "Oda servisi", "bi-bell"),
        ("24-hour front desk", "24 saat açık resepsiyon", "bi-clock"),
        ("fitness", "Fitness merkezi", "bi-heart-pulse"),
        ("spa", "Spa ve sağlıklı yaşam merkezi", "bi-flower1"),
        ("sauna", "Sauna", "bi-thermometer-high"),
        ("pool", "Yüzme havuzu", "bi-water"),
        ("restaurant", "Restoran", "bi-egg-fried"),
        ("barbecue", "Barbekü", "bi-fire"),
        ("bar", "Bar", "bi-cup-straw"),
        ("breakfast", "Kahvaltı", "bi-cup-hot"),
        ("tea/coffee", "Çay/kahve makinesi", "bi-cup-hot"),
        ("free parking", "Ücretsiz otopark", "bi-p-circle"),
        ("parking", "Otopark", "bi-p-circle"),
        ("electric vehicle", "Elektrikli araç şarj istasyonu", "bi-ev-station"),
        ("air conditioning", "Klima", "bi-snow"),
        ("heating", "Isıtma", "bi-thermometer-sun"),
        ("elevator", "Asansör", "bi-arrow-down-up"),
        ("lift", "Asansör", "bi-arrow-down-up"),
        ("disabled", "Engelli misafirler için olanaklar", "bi-universal-access"),
        ("pets", "Evcil hayvan kabul edilir", "bi-heart"),
        ("laundry", "Çamaşırhane", "bi-basket"),
        ("luggage", "Bagaj muhafazası", "bi-suitcase2"),
        ("housekeeping", "Günlük temizlik", "bi-stars"),
        ("terrace", "Teras", "bi-brightness-high"),
        ("garden", "Bahçe", "bi-tree"),
        ("beach", "Plaj", "bi-umbrella"),
        ("kitchen", "Mutfak", "bi-house")
    };

    public static FacilityItem Translate(string englishName)
    {
        var lower = englishName.ToLowerInvariant();

        foreach (var rule in Rules)
        {
            if (lower.Contains(rule.Keyword))
                return new FacilityItem(rule.Name, rule.Icon);
        }

        // Tabloda yoksa orijinal ismi göster
        return new FacilityItem(englishName, DefaultIcon);
    }

    // Puandan Türkçe değerlendirme yazısı üretir (Booking'in "Very good" metni yerine)
    public static string RatingText(double? score) => score switch
    {
        null => "Henüz puan yok",
        >= 9 => "Olağanüstü",
        >= 8 => "Çok İyi",
        >= 7 => "İyi",
        >= 6 => "Keyifli",
        _ => "Puan"
    };
}