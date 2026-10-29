using System.Text.Json.Serialization;

namespace Hotels.Web.Models.Api
{
    //API nin gönderdiği veriler
    public class ApiResponse<T> // endpointer { "status": true, "message": "...", "data": ... } böyle dönüyor
    {
        [JsonPropertyName("status")] public bool Status { get; set; }
        [JsonPropertyName("message")] public string? Message { get; set; }
        [JsonPropertyName("data")] public T? Data { get; set; }
    }

    public class DestinationSearchDto
    {
        [JsonPropertyName("dest_id")] public string DestId { get; set; } = string.Empty;
        [JsonPropertyName("search_type")] public string SearchType { get; set; } = string.Empty;
        [JsonPropertyName("name")] public string? Name { get; set; }
    }
    public class HotelSearchDataDto
    {
        [JsonPropertyName("hotels")] public List<HotelItemDto> Hotels { get; set; } = new();
    }

    public class HotelItemDto
    {
        [JsonPropertyName("hotel_id")] public int HotelId { get; set; }
        [JsonPropertyName("property")] public HotelPropertyDto? Property { get; set; }
    }
    public class HotelPropertyDto
    {
        [JsonPropertyName("name")] public string? Name { get; set; }
        [JsonPropertyName("reviewScore")] public double? ReviewScore { get; set; }
        [JsonPropertyName("reviewScoreWord")] public string? ReviewScoreWord { get; set; }
        [JsonPropertyName("reviewCount")] public int? ReviewCount { get; set; }
        [JsonPropertyName("photoUrls")] public List<string>? PhotoUrls { get; set; }
        [JsonPropertyName("priceBreakdown")] public PriceBreakdownDto? PriceBreakdown { get; set; }
    }
    public class PriceBreakdownDto
    {
        [JsonPropertyName("grossPrice")] public PriceDto? GrossPrice { get; set; }
    }

    public class PriceDto
    {
        [JsonPropertyName("value")] public decimal? Value { get; set; }
        [JsonPropertyName("currency")] public string? Currency { get; set; }
    }
}
