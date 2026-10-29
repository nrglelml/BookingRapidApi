using Hotels.Web.Models.Api;
using Hotels.Web.Models.ViewModels;
using Hotels.Web.Options;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;

namespace Hotels.Web.Services
{
    public class BookingApiService : IBookingService
    {
        private const string DateFormat = "yyyy-MM-dd";

        private readonly HttpClient _http;
        private readonly RapidApiEndpoints _endpoints;
        private readonly ILogger<BookingApiService> _logger;

        public BookingApiService(HttpClient http, IOptions<RapidApiOptions> options, ILogger<BookingApiService> logger)
        {
            _http = http;
            _endpoints = options.Value.Endpoints;
            _logger = logger;
        }
        public async Task<Destination?> FindDestinationAsync(string city)
        {
            var cityName = city.Split(',')[0].Trim();
            var url = QueryHelpers.AddQueryString(_endpoints.SearchDestination, "query", cityName);
            var results = await GetAsync<List<DestinationSearchDto>>(url);
            var match = results?.FirstOrDefault(d => d.SearchType.Equals("city", StringComparison.OrdinalIgnoreCase))
                    ?? results?.FirstOrDefault();

            return match is null ? null : new Destination(match.DestId, match.SearchType, match.Name ?? cityName);
        }

        public async Task<HotelSearchResult> SearchHotelsAsync(Destination destination, HotelSearchViewModel search)
        {
            var inv = CultureInfo.InvariantCulture;

            var query = new Dictionary<string, string?>
            {
                ["dest_id"] = destination.DestId,
                ["search_type"] = destination.SearchType.ToUpperInvariant(),
                ["arrival_date"] = search.CheckIn.ToString(DateFormat, inv),
                ["departure_date"] = search.CheckOut.ToString(DateFormat, inv),
                ["adults"] = search.Adults.ToString(inv),
                ["room_qty"] = search.Rooms.ToString(inv),
                ["page_number"] = search.Page.ToString(inv),
                ["currency_code"] = search.Currency,
                ["languagecode"] = "tr",
                ["units"] = "metric"
            };

            // Example: Child 1 Age = 8 months Child 2 Age = 1 year Child 3 Age = 17 years Here is what the request parameter would look like: children_age: 0,1,17
            if (search.Children > 0)
                query["children_age"] = string.Join(",", Enumerable.Repeat(search.ChildAge, search.Children));


            var url = QueryHelpers.AddQueryString(_endpoints.SearchHotels, query);
            var data = await GetAsync<HotelSearchDataDto>(url);

           
            if (data?.Hotels is null)
                return new HotelSearchResult(new List<HotelCardViewModel>(), null);

             var hotels= data.Hotels //mapping
                .Where(h => h.Property is not null)
                .Select(h => new HotelCardViewModel
                {
                    Id = h.HotelId,
                    Name = h.Property!.Name ?? "İsimsiz otel",
                    //fotoğrafları büyütmek için
                    PhotoUrl = h.Property.PhotoUrls?.FirstOrDefault()?.Replace("square60", "max500"),
                    Price = h.Property.PriceBreakdown?.GrossPrice?.Value,
                    Currency = h.Property.PriceBreakdown?.GrossPrice?.Currency,
                    Rating = h.Property.ReviewScore,
                    RatingText = h.Property.ReviewScoreWord,
                    ReviewCount = h.Property.ReviewCount,
                    CityName = h.Property.CityName,
                    Stars = h.Property.Stars,
                    MapUrl =BuildMapUrl(h.Property.Latitude,h.Property.Longitude),
                })
                .ToList();
            return new HotelSearchResult(hotels, ParseTotalCount(data.Meta?.FirstOrDefault()?.Title)); // arama sonucu kaç taneyse onun için
        }

        //hatayı yakalamak için
        private async Task<T?> GetAsync<T>(string url)
        {
            HttpResponseMessage response;
            try
            {
                response = await _http.GetAsync(url);
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Zaman aşımı: {Url}", url);
                throw new BookingApiException("Booking servisi zamanında yanıt vermedi. Lütfen tekrar deneyin.", ex);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Bağlantı hatası: {Url}", url);
                throw new BookingApiException("Booking servisine bağlanılamadı. İnternet bağlantınızı kontrol edin.", ex);
            }

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                _logger.LogError("API hatası {Status}: {Url} -> {Body}", (int)response.StatusCode, url, body);

                throw response.StatusCode switch
                {
                    HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden
                        => new BookingApiException("API anahtarı geçersiz veya bu API'ye abone değilsiniz."),
                    HttpStatusCode.TooManyRequests
                        => new BookingApiException("API istek limiti doldu. Biraz sonra tekrar deneyin."),
                    _ => new BookingApiException("Booking servisinden geçerli bir yanıt alınamadı.")
                };
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<T>>();

            // API 200 dönse bile "status": false ile hata bildirebiliyor
            if (result is null || !result.Status)
            {
                _logger.LogWarning("API status=false: {Url} -> {Message}", url, result?.Message);
                throw new BookingApiException("Arama yapılamadı. Girdiğiniz bilgileri kontrol edip tekrar deneyin.");
            }

            return result.Data;
        } 

        private static int? ParseTotalCount(string? title)//... şehri için ... adet kayıt bulundu
        {
            if (string.IsNullOrWhiteSpace(title))
                return null;

            var match = Regex.Match(title, @"\d[\d.,]*");
            if (!match.Success)
                return null;

            var digits = match.Value.Replace(".", "").Replace(",", "");
            return int.TryParse(digits, out var total) ? total : null;
        }
        private static string? BuildMapUrl(double? latitude, double? longitude) =>
    latitude is not null && longitude is not null
        ? string.Create(CultureInfo.InvariantCulture, $"https://www.google.com/maps?q={latitude},{longitude}")
        : null;


        public async Task<HotelDetailViewModel?> GetHotelDetailsAsync(int hotelId, HotelSearchViewModel search)
        {
            var inv = CultureInfo.InvariantCulture;

            var query = new Dictionary<string, string?>
            {
                ["arrival_date"] = search.CheckIn!.ToString(DateFormat, inv),
                ["departure_date"] = search.CheckOut!.ToString(DateFormat, inv),
                ["adults"] = search.Adults.ToString(inv),
                ["room_qty"] = search.Rooms.ToString(inv),
                ["currency_code"] = search.Currency,
                ["languagecode"] = "tr",
                ["hotel_id"] = hotelId.ToString(inv)
            };
            var detailsUrl = QueryHelpers.AddQueryString(_endpoints.HotelDetails, query);
           
            var detailsTask=GetAsync<HotelDetailsDto>(detailsUrl);
            var photosTask = GetPhotosAsync(hotelId);
            await Task.WhenAll(detailsTask, photosTask);

            var details = await detailsTask;
            if(details is null)
                return null;

            return new HotelDetailViewModel //mapping
            {
                Id = details.HotelId,
                Name = details.HotelName,
                Address = details.Address,
                City = details.City,
                Country = details.Country,
                Rating = details.RawData?.ReviewScore,
                Price = details.PriceBreakdown?.GrossAmount?.Value,
                Currency = details.PriceBreakdown?.GrossAmount?.Currency,
                MapUrl = BuildMapUrl(details.Latitude, details.Longitude),
                Photos = await photosTask,
                Facilities = details.FacilitiesBlock?.Facilities?.Where(f => !string.IsNullOrWhiteSpace(f.Name)).Select(f => f.Name!).Distinct().ToList() ?? new List<string>()
            };

        }
        private async Task<List<string>> GetPhotosAsync(int id)
        {
            try
            {
                var url = QueryHelpers.AddQueryString(_endpoints.HotelPhotos, "hotel_id",id.ToString(CultureInfo.InvariantCulture));
                var photos = await GetAsync<List<HotelPhotoDto>>(url);
                return photos?.Where(p => !string.IsNullOrWhiteSpace(p.Url)).Select(p => p.Url!).Take(15).ToList() ?? new List<string>();
            }
            catch (BookingApiException ex)
            {
                _logger.LogWarning(ex, "Otel fotoğrafları alınamadı. HotelId: {HotelId}", id);
                return new List<string>();
            }
        }
    }
}
