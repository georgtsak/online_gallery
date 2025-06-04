using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineGallery.Data;
using OnlineGallery.Models;
using Supabase;
using Supabase.Gotrue;
using Microsoft.EntityFrameworkCore;
using OnlineGallery.Helper;

namespace OnlineGallery.Controllers
{
    public class ArtworksController : Controller
    {
        private readonly AppDbContext _context;
        private readonly Supabase.Client _supabaseClient;

        public ArtworksController(AppDbContext context, Supabase.Client supabaseClient)
        {
            _context = context;
            _supabaseClient = supabaseClient;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateArtworkRequest request)
        {
            var UserId = HttpContext.Session.GetInt32("UserId");
            if (UserId == null)
                return RedirectToAction("Login", "Users");

            if (!ModelState.IsValid)
                return View(request);

            using var memoryStream = new MemoryStream();
            await request.Image.CopyToAsync(memoryStream);
            var imageBytes = memoryStream.ToArray();

            var bucket = _supabaseClient.Storage.From("artworks");
            var fileName = $"{Guid.NewGuid()}_{request.Image.FileName}";
            await bucket.Upload(imageBytes, fileName);
            var publicUrl = bucket.GetPublicUrl(fileName);

            var artwork = new ArtworksModel
            {
                Title = request.Title,
                Description = request.Description,
                Price = request.Price,
                ArtistId = UserId.Value,
                ImageUrl = publicUrl,
            };

            _context.Artworks.Add(artwork);
            await _context.SaveChangesAsync();

            UserRoleHelper.UpdateUserRole(_context, UserId.Value); // helper

            return RedirectToAction("Profile","Users");
        }

        // **************************************************** return View ******
        public IActionResult Create()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
                //return RedirectToAction("Login", "Users", new { returnUrl = "/Artworks/Create" });
            }
            return View();
        }

        // ********************************************************** index ******

        public async Task<IActionResult> Index()
        {
            var artworks = await _context.Artworks
                .Include(a => a.Artist)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            var userId = HttpContext.Session.GetInt32("UserId");
            var buyer = await _context.Users.FindAsync(userId);

            bool isBuyerBanned = buyer != null && UserHelper.IsUserBanned(_context, buyer.Id);
            ViewBag.IsBuyerBanned = isBuyerBanned;

            var artistBanStatus = new Dictionary<int, bool>();
            foreach (var artwork in artworks)
            {
                if (artwork.Artist != null && !artistBanStatus.ContainsKey(artwork.Artist.Id))
                {
                    artistBanStatus[artwork.Artist.Id] = UserHelper.IsUserBanned(_context, artwork.Artist.Id);
                }
            }
            ViewBag.ArtistBanStatus = artistBanStatus;
            ViewBag.UserId = userId;

            return View(artworks);
        }


        // *************************************************** edit artwork ******

        public async Task<IActionResult> Edit(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Users");

            var artwork = await _context.Artworks.FindAsync(id);
            if (artwork == null)
                return NotFound();
            if (artwork.ArtistId != userId.Value)
                return RedirectToAction("Index");

            ViewBag.IsEdit = true;

            return View("Create", artwork);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ArtworksModel updatedArtwork)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Users");

            var artwork = await _context.Artworks.FindAsync(id);
            if (artwork == null)
                return NotFound();

            if (artwork.ArtistId != userId.Value)
                return RedirectToAction("Index");

            artwork.Title = updatedArtwork.Title;
            artwork.Description = updatedArtwork.Description;
            artwork.Price = updatedArtwork.Price;

            _context.Update(artwork);
            await _context.SaveChangesAsync();

            return RedirectToAction("Profile", "Users");
        }

        // ************************************************* delete artwork ******

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Users");

            var artwork = await _context.Artworks.FindAsync(id);
            if (artwork == null)
                return NotFound();

            if (!string.IsNullOrEmpty(artwork.ImageUrl)) // diagrafh apo th supabase
            {
                var uri = new Uri(artwork.ImageUrl);
                var fileName = Path.GetFileName(uri.LocalPath);

                var bucket = _supabaseClient.Storage.From("artworks");
                await bucket.Remove(fileName);
            }

            _context.Artworks.Remove(artwork);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


    }
}