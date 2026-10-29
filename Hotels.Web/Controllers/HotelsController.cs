using Hotels.Web.Models.ViewModels;
using Hotels.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hotels.Web.Controllers
{
    public class HotelsController(IBookingService _bookingService) : Controller
    {

    
        public async Task<IActionResult> Index(HotelSearchViewModel search)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Home/Index.cshtml", search);


            var model = new HotelListViewModel { Search = search };
            try
            {
                var dest = await _bookingService.FindDestinationAsync(search.City);
                if(dest is null)
                {
                    model.ErrorMessage= $"\"{search.City}\" için bir sonuç bulunamadı. Yazımı kontrol edip tekrar deneyin.";
                }
                model.DestinationName = dest.Name;
                var result= await _bookingService.SearchHotelsAsync(dest, search);
                model.Hotels = result.Hotels;
                model.TotalCount = result.TotalCount;
            }
            catch (BookingApiException ex)
            {
                model.ErrorMessage = ex.Message;
            }

            return View(model);
        }
        public async Task<IActionResult> Details(int id, HotelSearchViewModel search)
        {
            if (!ModelState.IsValid)
            {
                return View(new HotelDetailViewModel
                {
                    Search = search,
                    ErrorMessage = "Arama bilgileri eksik veya hatalı. Lütfen yeni bir arama yapın."
                });
            }
            try
            {
                var hotel = await _bookingService.GetHotelDetailsAsync(id, search);
                if(hotel is null)
                {
                    return View(new HotelDetailViewModel { Search = search, ErrorMessage = "Otel bulunamadı" });
                }
                hotel.Search= search;
                return View(hotel);
            }
            catch(BookingApiException ex)
            {
                return View(new HotelDetailViewModel
                {
                    Search = search,
                    ErrorMessage = ex.Message
                });
            }

        }


    }
}