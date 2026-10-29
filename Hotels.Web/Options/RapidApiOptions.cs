namespace Hotels.Web.Options
{
    public class RapidApiOptions
    {
        public const string SectionName = "RapidApi";

        public string BaseUrl { get; set; } = string.Empty;
        public int TimeoutSeconds { get; set; } = 15;

  
        public Dictionary<string, string> Headers { get; set; } = new(); //key ve host bilgileri

        public RapidApiEndpoints Endpoints { get; set; } = new();
    }

    public class RapidApiEndpoints
    {
        public string SearchDestination { get; set; } = string.Empty;
        public string SearchHotels { get; set; } = string.Empty;
        public string HotelDetails { get; set; } = string.Empty;

    }
}
