using System.Text.Json.Serialization;

namespace Hotels.Web.Models.Api
{
    //API nin gönderdiği veriler
    public class ApiResponse<T> // endpointer { "status": true, "message": "...", "data": ... } böyle dönüyor data da hotels,meta olarak dönüyor meta= title:"198 properties around Chitota"
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
        [JsonPropertyName("meta")] public List<SearchMetaDto>? Meta { get; set; }
    }
    public class SearchMetaDto
    {
        [JsonPropertyName("title")] public string? Title { get; set; }
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
        [JsonPropertyName("wishlistName")] public string? CityName { get; set; }
        [JsonPropertyName("latitude")] public double? Latitude { get; set; }
        [JsonPropertyName("longitude")] public double? Longitude { get; set; }
        [JsonPropertyName("propertyClass")] public int? Stars { get; set; }
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

    public class HotelDetailsDto
    {
        [JsonPropertyName("hotel_id")] public int HotelId { get; set; }
        [JsonPropertyName("hotel_name")] public string? HotelName { get; set; }
        [JsonPropertyName("address")] public string? Address { get; set; }
        [JsonPropertyName("city")] public string? City { get; set; }
        [JsonPropertyName("country_trans")] public string? Country { get; set; }
        [JsonPropertyName("review_nr")] public int? ReviewCount { get; set; }
        [JsonPropertyName("rawData")] public HotelRawDataDto? RawData { get; set; }
        [JsonPropertyName("facilities_block")] public FacilitiesBlockDto? FacilitiesBlock { get; set; }
        [JsonPropertyName("product_price_breakdown")] public ProductPriceBreakdownDto? PriceBreakdown { get; set; }
        [JsonPropertyName("latitude")] public double? Latitude { get; set; }
        [JsonPropertyName("longitude")] public double? Longitude { get; set; }
    }

    public class HotelRawDataDto
    {
        [JsonPropertyName("reviewScore")] public double? ReviewScore { get; set; }
        [JsonPropertyName("reviewScoreWord")] public string? ReviewScoreWord { get; set; }
    }

    public class FacilitiesBlockDto
    {
        [JsonPropertyName("facilities")] public List<FacilityDto>? Facilities { get; set; }
    }

    public class FacilityDto
    {
        [JsonPropertyName("name")] public string? Name { get; set; }
    }

    public class ProductPriceBreakdownDto
    {
        [JsonPropertyName("gross_amount")] public PriceDto? GrossAmount { get; set; }
    }

    public class HotelPhotoDto
    {
        [JsonPropertyName("url")] public string? Url { get; set; }
    }
}
