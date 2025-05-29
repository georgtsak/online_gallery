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
            var model = new HomeModel
            {
                TopArtists = GetTopArtists(),
                RecentArtworks = GetRecentArtworksOfTopArtists()
            };

            return View(model);
        }

        // *********************************** top 5 artists based on sales ******

        private List<HomeModel.TopArtists1> GetTopArtists()
        {
            var topArtistsData = _context.Transactions
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
                .ToList();

            var topArtists = topArtistsData
                    .Select(x => new HomeModel.TopArtists1
                    {
                        Artist = x.Artist,
                        SalesCount = x.SalesCount
                    })
                    .ToList();

            return topArtists;
        }

        // ********************************* recent artworks of top artists ******

        private List<ArtworksModel> GetRecentArtworksOfTopArtists()
        {
            var topArtistIds = _context.Transactions
                .Where(t => t.Status == TransactionStatus.Completed)
                .Include(t => t.Artwork)
                .GroupBy(t => t.Artwork.ArtistId)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => g.Key)
                .ToList();

            var artworks = _context.Artworks
                .Where(a => topArtistIds.Contains(a.ArtistId))
                .OrderByDescending(a => a.CreatedAt)
                .Take(20)
                .ToList();

            return artworks;
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
