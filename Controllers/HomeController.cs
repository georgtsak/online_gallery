using OnlineGallery.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using OnlineGallery.Data;

namespace OnlineGallery.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var topArtists = _context.Transactions
                .Where(t => t.Status == TransactionStatus.Completed)
                .Include(t => t.Artwork)
                .ThenInclude(a => a.Artist)
                .GroupBy(t => t.Artwork.ArtistId)
                .Select(g => new
                {
                    Artist = g.First().Artwork.Artist,
                    ArtistId = g.Key,
                    SalesCount = g.Count()
                })
                .OrderByDescending(x => x.SalesCount)
                .Take(5)
                .ToList()
                .Select(x =>
                {
                    var sampleArtwork = _context.Artworks
                        .Where(a => a.ArtistId == x.ArtistId)
                        .OrderBy(r => Guid.NewGuid())
                        .FirstOrDefault();

                    return new HomeModel.TopArtists1
                    {
                        Artist = x.Artist,
                        SalesCount = x.SalesCount,
                        SampleArtwork = sampleArtwork
                    };
                })
                .ToList();

            var model = new HomeModel
            {
                TopArtists = topArtists
            };

            return View(model);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
