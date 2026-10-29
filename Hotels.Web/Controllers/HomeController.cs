using Hotels.Web.Models;
using Hotels.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Hotels.Web.Controllers
{
    public class HomeController : Controller
    {

          public IActionResult Index() => View(new HotelSearchViewModel());
        
    }
}
