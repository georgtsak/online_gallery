using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineGallery.Data;
using OnlineGallery.Helper;
using OnlineGallery.Models;
using Supabase.Gotrue;
using System.Diagnostics;
using System.Security.Claims;

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
                RecentArtworks = GetRecentArtworksOfTopArtists(),
                RecommendedArtworks = GetRecommendedArtworks(),
                ArtistProfile = GetArtistProfile()
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
                // exclude banned users from top5
                //.ToList()
                //.Where(x => !UserHelper.IsUserBanned(_context, x.ArtistId))
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

        // ********************************** recommended artworks for user ******

        private List<ArtworksModel> GetRecommendedArtworks()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return new List<ArtworksModel>();
            }

            // completed transactions
            var artistIds = _context.Transactions
                .Where(t => t.BuyerId == userId && t.Status == TransactionStatus.Completed)
                .Include(t => t.Artwork)
                .Select(t => t.Artwork.ArtistId)
                .Distinct()
                .ToList();

            if (!artistIds.Any())
            {
                return new List<ArtworksModel>();
            }

            var recommended = _context.Artworks
                .Where(a => artistIds.Contains(a.ArtistId) && a.Status == ArtworkStatus.Available)
                .OrderByDescending(a => a.CreatedAt)
                .Take(8)
                .ToList();

            return recommended;
        }

        // ******************************************* artist profile modal ******

        private List<ArtistModalModel> GetArtistProfile()
        {
            var topArtistIds = GetTopArtists().Select(a => a.Artist.Id).ToList();

            var users = _context.Users
                .Where(u => topArtistIds.Contains(u.Id))
                .ToList();

            var artistProfiles = users.Select(u => new ArtistModalModel
            {
                Artist = u,
                IsBanned = UserHelper.IsUserBanned(_context, u.Id),
                Artworks = _context.Artworks
                    .Where(a => a.ArtistId == u.Id)
                    .OrderByDescending(a => a.CreatedAt)
                    .ToList()
            })
            .ToList();

            return artistProfiles;
        }


        // ********************************** ***************************** ******

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
