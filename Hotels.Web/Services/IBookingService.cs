using Hotels.Web.Models.ViewModels;

namespace Hotels.Web.Services
{
    public interface IBookingService
    {
        Task<Destination?> FindDestinationAsync(string city);
        Task<HotelSearchResult> SearchHotelsAsync(Destination destination, HotelSearchViewModel search);
        Task<HotelDetailViewModel?> GetHotelDetailsAsync(int hotelId, HotelSearchViewModel search);
    }
}
