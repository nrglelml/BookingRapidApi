StayFinder – Otel Arama

Booking.com API'si (RapidAPI) üzerinden otel arayan, sonuçları listeleyen ve seçilen otelin detaylarını gösteren bir ASP.NET Core 8 MVC uygulaması.

Kullanıcı şehir, tarih, kişi sayısı ve para birimini seçer. Uygulama önce şehrin Booking'deki kimliğini bulur, sonra o şehirdeki müsait otelleri listeler. Bir otele tıklandığında fotoğraflar, olanaklar, açıklama ve harita konumuyla detay sayfası açılır.

Ana sayfa

İçindekiler
Özellikler
Ekran Görüntüleri
Kullanılan Teknolojiler
Kurulum
API Anahtarı Olmadan Çalıştırma (Mock Mod)
Mimari
Kullanılan Endpoint'ler
Hata Yönetimi
Proje Yapısı
Tasarım Kararları ve Bilinen Sınırlar
Özellikler

Arama (ana sayfa)

Şehir, giriş-çıkış tarihi, yetişkin, çocuk, oda sayısı, çocuk yaşı ve para birimi (EUR, USD, TRY, GBP) ile arama
Sunucu tarafı doğrulama: boş şehir, geçmiş tarih, çıkışın girişten önce olması gibi durumlarda Türkçe hata mesajları
Arama GET ile yapılır; sonuç sayfası yenilenebilir, paylaşılabilir ve tarayıcının geri tuşuyla bozulmaz

Otel listesi

Otel adı, fotoğrafı, yıldız sayısı, misafir puanı, değerlendirme sayısı ve toplam fiyat
API tarafında sıralama: önerilen, en düşük fiyat, en yüksek puan, yıldız sayısı
Sayfalama (sayfa başına 20 otel); sıralama ve arama bilgileri sayfalar arasında korunur
Toplam sonuç sayısı, sonuç bulunamadı ve hata durumları için ayrı ekranlar

Otel detayı

Fotoğraf galerisi ve kaydırılabilir fotoğraf alanı
Otel açıklaması (Türkçesi yoksa İngilizce)
Türkçeleştirilmiş olanak listesi
Google Haritalar ile konum
Seçilen tarihler, misafir sayısı ve toplam fiyatı gösteren konaklama özeti
"Sonuçlara dön" bağlantısı kullanıcıyı listede kaldığı sayfaya ve sıralamaya geri götürür
Ekran Görüntüleri
Ana sayfa	
<img width="%100" alt="Ana Sayfa" src="https://github.com/user-attachments/assets/78e2a979-f890-40be-92bb-9dfa63521a22" />

Ana sayfa	Otel listesi
<img width="%100" alt="Otel Listesi" src="https://github.com/user-attachments/assets/2280ecc5-fea6-42e0-a5ea-2e60480c5609" />

Otel Detayı Sayfaları
<img width="%100" alt="Otel Galerisi" src="https://github.com/user-attachments/assets/ae56c77d-b690-4332-8fa5-bdcdbf99b547" />
<img width="%100" alt="Otel Açıklaması" src="https://github.com/user-attachments/assets/9c99f013-2584-49d1-84cb-3446ba24b806" />
<img width="%100" alt="Harita ve özellikler" src="https://github.com/user-attachments/assets/60191451-b904-415d-8f89-e3532ea22281" />
<img width="%100" alt="Otel Görselleri" src="https://github.com/user-attachments/assets/6562eb8c-b44f-4554-b669-c9aea75f9c19" />

<details> <summary>Hata Sayfaları</summary>
<img width="%100" alt="İnternet bağlantısı hatası" src="https://github.com/user-attachments/assets/daa89c49-baed-4e1b-ba01-aff96a33d02c" />
<img width="%100 alt="Api istek limit hatası" src="https://github.com/user-attachments/assets/c88a54b6-58ee-4a54-a9f8-af5ae1cd5043" />

</details>
Kullanılan Teknolojiler
Uygulama	ASP.NET Core 8 MVC, C# 12
Görünüm	Razor View, Tag Helper, Partial View
Arayüz	Bootstrap 5, Bootstrap Icons
HTTP	IHttpClientFactory ile typed HttpClient
JSON	System.Text.Json
Doğrulama	Data Annotations + IValidatableObject
Ayarlar	Options Pattern (IOptions<RapidApiOptions>)
Loglama	ILogger
Veri kaynağı	Booking.com API – DataCrawler (RapidAPI)

Sayfalar sunucuda üretilir. JavaScript yalnızca ana sayfadaki kişi/oda sayaçlarında, sıralama kutusunun otomatik gönderiminde ve temanın fotoğraf kaydırıcısında kullanılır.

Kurulum
Gereksinimler
.NET 8 SDK
Visual Studio 2022, Rider veya VS Code
Gerçek veriyle çalıştırmak için RapidAPI hesabı ve Booking.com API aboneliği (ücretsiz plan yeterli)
1. Projeyi indirin
bash
git clone <https://github.com/nrglelml/BookingRapidApi>
cd Hotels.Web
2. API anahtarını girin

API anahtarı güvenlik nedeniyle repoda yer almaz. Proje klasöründe appsettings.Copy.Development.json dosyasını kendi projenize aktarın ve API key girin.


Bu dosya .gitignore içinde olduğu için yanlışlıkla repoya gönderilmez. Uygulama başlarken appsettings.json ve bu dosyayı otomatik olarak birleştirir.


Konsolda yazan adresi tarayıcıda açın (ör. https://localhost:7266). Visual Studio kullanıyorsanız projeyi açıp F5 ile başlatabilirsiniz.

API Anahtarı Olmadan Çalıştırma (Mock Mod)

API anahtarınız yoksa veya aylık istek kotası dolduysa, uygulama örnek veriyle de çalışabilir. appsettings.Development.json dosyasında:

json
{
  "RapidApi": {
    "UseMock": true
  }
}

Bu modda istekler internete çıkmaz; MockBookingApiHandler gerçek API ile aynı yapıda cevaplar döndürür (45 otel, 3 sayfa, fotoğraflar, olanaklar, açıklama). Servis katmanı hiç değişmeden çalıştığı için URL oluşturma, JSON okuma ve hata yönetimi de bu modda test edilmiş olur.

Hata ekranlarını denemek için şehir alanına şunları yazabilirsiniz:

Şehir alanına	Sonuç
yok	"Şehir bulunamadı" mesajı
hata	Sunucu hatası mesajı
limit	İstek limiti doldu mesajı
yetki	Geçersiz API anahtarı mesajı


Mimari
Tarayıcı
   │  GET /Hotels?City=Milano&CheckIn=...
   ▼
HotelsController          → formu doğrular, servisi çağırır, view model'i doldurur
   │
   ▼
IBookingService           → controller API'yi tanımaz, sadece bu arayüzü bilir
   │
   ▼
BookingApiService         → URL'leri kurar, isteği atar, hataları yakalar,
   │                         API cevabını view model'e çevirir
   ▼
HttpClient ──► RapidAPI (veya MockBookingApiHandler)
   │
   ▼
Razor View + Partial View → veriyi temaya yerleştirir

Her katman tek bir iş yapar:

Controller HTTP ve JSON detaylarını bilmez.
Servis API'nin karmaşık cevabını, view'ların ihtiyaç duyduğu sade sınıflara (HotelCardViewModel, HotelDetailsViewModel) çevirir. API'nin cevap yapısı değişirse sadece servis ve Models/Api sınıfları güncellenir; view'lar etkilenmez.
HttpClient Program.cs içinde bir kez yapılandırılır: adres, header'lar ve zaman aşımı.
Arama akışı
Kullanıcı formu gönderir; değerler URL'e eklenir.
HotelSearchViewModel doğrulanır. Hata varsa form, mesajlarla birlikte tekrar gösterilir.
searchDestination ile şehrin dest_id değeri bulunur.
searchHotels ile o şehirdeki müsait oteller alınır.
Sonuçlar _HotelList partial'ı ile listelenir.
Detay akışı
Kart bağlantısı otel id'sini ve bütün arama bilgilerini taşır.
getHotelDetails, getHotelPhotos ve getDescriptionAndInfo istekleri aynı anda (Task.WhenAll) gönderilir.
Fotoğraf veya açıklama alınamazsa sayfa yine açılır, sadece o bölüm gizlenir.
Kullanılan Endpoint'ler
Endpoint	Ne için	Önemli parametreler
api/v1/hotels/searchDestination	Şehir adından dest_id bulmak	query
api/v1/hotels/searchHotels	Otel listesi	dest_id, search_type, arrival_date, departure_date, adults, children_age, room_qty, currency_code, page_number, sort_by
api/v1/hotels/getHotelDetails	Otel bilgileri, olanaklar, fiyat, konum	hotel_id + konaklama parametreleri
api/v1/hotels/getHotelPhotos	Fotoğraflar	hotel_id
api/v1/hotels/getDescriptionAndInfo	Otel açıklaması	hotel_id, languagecode

Endpoint yolları appsettings.json içindeki RapidApi:Endpoints bölümünde tutulur; API değişirse kodu derlemeden güncellenebilir.

Hata Yönetimi

Bütün istekler BookingApiService içindeki tek bir GetAsync metodundan geçer. Bu metot hataları yakalar ve kullanıcıya gösterilebilecek Türkçe bir mesaj taşıyan BookingApiException'a çevirir. Controller bu hatayı yakalayıp ilgili sayfada gösterir; uygulama çökmez.

Durum	Kullanıcının gördüğü
Boş şehir, geçmiş tarih, çıkış ≤ giriş	İlgili alanın altında doğrulama mesajı
Şehir bulunamadı	"… için bir sonuç bulunamadı"
Bu tarihlerde otel yok	Boş sonuç ekranı ve yeni arama bağlantısı
401 / 403	"API anahtarı geçersiz veya bu API'ye abone değilsiniz"
429	"API istek limiti doldu"
5xx	"Booking servisinden geçerli bir yanıt alınamadı"
Zaman aşımı (15 sn)	"Booking servisi zamanında yanıt vermedi"
Bağlantı yok	"Booking servisine bağlanılamadı"
API status: false döndü	"Arama yapılamadı…"
Beklenmeyen JSON yapısı	"Booking servisinden beklenmeyen bir yanıt geldi"
Geçersiz otel bağlantısı	Detay sayfasında hata mesajı, API'ye istek gönderilmez

Teknik ayrıntılar (URL, durum kodu, API'nin ham cevabı) kullanıcıya gösterilmez, ILogger ile loglanır. Adres çubuğundan elle girilen geçersiz değerler (ör. Currency=XYZ, SortBy=abc) API'ye gönderilmeden doğrulamada yakalanır.

Proje Yapısı
HotelSearch/
├─ Controllers/
│  ├─ HomeController.cs          Ana sayfa ve arama formu
│  └─ HotelsController.cs        Otel listesi (Index) ve detay (Details)
├─ Services/
│  ├─ IBookingService.cs
│  ├─ BookingApiService.cs       API istekleri, eşleştirme ve hata yönetimi
│  ├─ BookingApiException.cs
│  
├─ Models/
│  ├─ Api/                       API'den gelen JSON'un C# karşılıkları
│  ├─ ViewModels/                View'ların kullandığı sınıflar
│  └─ FacilityTranslator.cs      Olanak isimlerinin Türkçesi ve ikonları
├─ Options/
│  └─ RapidApiOptions.cs         appsettings'teki RapidApi bölümü
├─ Views/
│  ├─ Home/Index.cshtml
│  ├─ Hotels/Index.cshtml
│  ├─ Shared/_HotelList.cshtml   Sonuç kartları, boş/hata durumları, sayfalama
│  ├─ Hotels/Details.cshtml
│  └─ Shared/_Layout.cshtml
├─ wwwroot/                      Tema dosyaları (CSS, JS, görseller)
├─ appsettings.json              API adresi ve endpoint yolları (anahtar yok)
└─ Program.cs                    Servis kayıtları ve HttpClient yapılandırması
Tasarım Kararları ve Bilinen Sınırlar
API isteklerini az tutmak: Ücretsiz planın aylık istek limiti düşük. Liste sayfası yalnızca arama sonucundaki verileri kullanır; açıklama ve olanaklar her otel için ayrı istek gerektirdiğinden sadece detay sayfasında gösterilir.
Çocuk yaşı: API her çocuğun yaşını ayrı ister, arayüzde ise tek bir yaş alanı var. Seçilen yaş bütün çocuklara uygulanır (2 çocuk, 5 yaş → 5,5).
Fiyat: API'nin döndürdüğü fiyat, seçilen tarihler için toplam konaklama fiyatıdır; gecelik değildir. Vergi ve ücret ayrıntısı gösterilmez.
Dil: Booking olanak isimlerini yalnızca İngilizce döndürüyor. Sık görülen olanaklar Türkçeye çevrilir; çevirisi olmayanlar İngilizce gösterilir. Misafir puanı yazısı ("Çok İyi" vb.) API'nin İngilizce metni yerine puandan üretilir. Türkçe açıklaması olmayan otellerde İngilizce açıklama gösterilir.
Sıralama: Sıralama API'ye yaptırılır. Uygulamada aynı anda sadece bir sayfa (20 otel) bulunduğu için kendi tarafımızda sıralamak yanlış sonuç verirdi.
Para birimleri: Sabit bir listeden (EUR, USD, TRY, GBP) gelir. API'den çekmek her sayfa açılışında ek bir istek demek olurdu.
Harita: Google Maps'in API anahtarı gerektirmeyen gömme adresiyle gösterilir.
Kapsam dışı: Rezervasyon, oda seçimi, filtre paneli (yıldız, olanak, bölge) ve misafir yorumları eklenmedi.
