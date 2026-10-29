using Hotels.Web.Models.Api;
using Hotels.Web.Models.ViewModels;
using Hotels.Web.Options;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Net;

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

        public async Task<List<HotelCardViewModel>> SearchHotelsAsync(Destination destination, HotelSearchViewModel search)
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
                return new List<HotelCardViewModel>();

            return data.Hotels
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
                    ReviewCount = h.Property.ReviewCount
                })
                .ToList();
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
    }
}
