using Microsoft.AspNetCore.Mvc;

namespace OnlineGallery.Controllers
{
    public class GalleryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
