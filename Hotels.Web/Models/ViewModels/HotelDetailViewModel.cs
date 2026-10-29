namespace Hotels.Web.Models.ViewModels
{
    public class HotelDetailViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public double? Rating { get; set; }
        public string? RatingText { get; set; }
        public int? ReviewCount { get; set; }
        public decimal? Price { get; set; }
        public string? Currency { get; set; }
        public List<string> Photos { get; set; } = new();
        public List<string> Facilities { get; set; } = new();
        public string? ErrorMessage { get; set; }
        public string? MapUrl { get; set; }
        public HotelSearchViewModel Search { get; set; } = new(); //geri dönmek için arama bilgileri
    }
}
