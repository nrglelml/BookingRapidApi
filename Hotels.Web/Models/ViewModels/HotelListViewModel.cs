namespace Hotels.Web.Models.ViewModels
{
    /*
     public record; içindeki veriler aynı olduğunda birbirine eşit kabul edilen, değerleri sonradan değiştirilemeyen (immutable) ve minimal kodla veri taşımak için optimize edilmiş özel bir sınıf (class) türü tanımlar.
     */
    public record Destination(string DestId, string SearchType, string Name);
    public record HotelSearchResult(List<HotelCardViewModel> Hotels, int? TotalCount);
    public class HotelCardViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; }
        public decimal? Price { get; set; }
        public string? Currency { get; set; }
        public double? Rating { get; set; }
        public string? RatingText { get; set; }
        public int? ReviewCount { get; set; }
        public string? CityName { get; set; }
        public int? Stars { get; set; }
        public string? MapUrl { get; set; }
    }
    public class HotelListViewModel
    {
        public HotelSearchViewModel Search { get; set; } = new();
        public string? DestinationName { get; set; }
        public List<HotelCardViewModel> Hotels { get; set; } = new();
        public string? ErrorMessage { get; set; }


        public int? TotalCount { get; set; }
        //paging
        public const int PageSize = 20;   // API'nin sayfa başına döndürdüğü otel sayısı

        public int CurrentPage => Search.Page;
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => TotalCount is not null
    ? LastIndex < TotalCount
    : Hotels.Count >= PageSize;
        public int FirstIndex => (CurrentPage - 1) * PageSize + 1;
        public int LastIndex => FirstIndex + Hotels.Count - 1;

    
       
    }
}
