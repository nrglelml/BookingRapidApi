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
                return RedirectToAction("Index", "Home",search);
             

            var model = new HotelListViewModel { Search = search };
            try
            {
                var dest = await _bookingService.FindDestinationAsync(search.City);
                if(dest is null)
                {
                    model.ErrorMessage= $"\"{search.City}\" için bir sonuç bulunamadı. Yazımı kontrol edip tekrar deneyin.";
                }
                model.DestinationName = dest.Name;
                model.Hotels = await _bookingService.SearchHotelsAsync(dest, search);
            }
            catch (BookingApiException ex)
            {
                model.ErrorMessage = ex.Message;
            }

            return View(model);
        }



    }
}