using System.ComponentModel.DataAnnotations;

namespace Hotels.Web.Models.ViewModels
{
    //Kullanıcının gönderdiği veriler
    public class HotelSearchViewModel
    {
        [Required(ErrorMessage = "Lütfen gideceğiniz yeri yazın.")]
        public string City { get; set; } = string.Empty;

        [DataType(DataType.Date)] // otomatik yyyy-mm-dd olur api böyle 
        public DateTime CheckIn { get; set; } = DateTime.Today.AddDays(1);

        [DataType(DataType.Date)]
        public DateTime CheckOut { get; set; } = DateTime.Today.AddDays(3);

        public string Currency { get; set; } = "EUR";

        [Range(1, 30, ErrorMessage = "Yetişkin sayısı 1 ile 30 arasında olmalı.")]
        public int Adults { get; set; } = 2;

        [Range(0, 10, ErrorMessage = "Çocuk sayısı 0 ile 10 arasında olmalı.")]
        public int Children { get; set; } = 0;

        [Range(1, 10, ErrorMessage = "Oda sayısı 1 ile 10 arasında olmalı.")]
        public int Rooms { get; set; } = 1;

        [Range(0, 17, ErrorMessage = "Çocuk yaşı 0 ile 17 arasında olmalı.")]
        public int ChildAge { get; set; } = 2;

        // apideki extra alanlar
        public decimal? PriceMin { get; set; }
        public decimal? PriceMax { get; set; }
        public string? SortBy { get; set; }
        public int Page { get; set; } = 1;

        public int Nights => (CheckOut.Date - CheckIn.Date).Days;

        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            if (CheckIn.Date < DateTime.Today)
                yield return new ValidationResult("Giriş tarihi geçmişte olamaz.", new[] { nameof(CheckIn) });

            if (CheckOut.Date <= CheckIn.Date)
                yield return new ValidationResult("Çıkış tarihi giriş tarihinden sonra olmalı.", new[] { nameof(CheckOut) });

            if (PriceMin is not null && PriceMax is not null && PriceMin > PriceMax)
                yield return new ValidationResult("En düşük fiyat, en yüksek fiyattan büyük olamaz.", new[] { nameof(PriceMin) });
            if (!Currencies.Supported.Contains(Currency))
                yield return new ValidationResult("Geçersiz para birimi.", new[] { nameof(Currency) });
        }
        public static class Currencies
        {
            public static readonly string[] Supported = { "EUR", "USD", "TRY", "GBP" };
        }
    }
}
