using Microsoft.AspNetCore.Mvc;

namespace OnlineGallery.Controllers
{
    public class ArtworksController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }
        

    }
}
