using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineGallery.Data;
using OnlineGallery.Models;
using Supabase;
using Supabase.Gotrue;

namespace OnlineGallery.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public async Task<IActionResult> UploadArtwork([FromForm] CreateArtworkRequest request)
        {
            var UserId = HttpContext.Session.GetInt32("UserId");

            if (_supabaseClient == null)
            {
                return StatusCode(500, "Supabase client not injected!");
            }

            Console.WriteLine("Uploading image...");
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
                ImageUrl = publicUrl
            };

            _context.Artworks.Add(artwork);
            await _context.SaveChangesAsync();

            return Ok(new { Url = publicUrl, Message = "Artwork uploaded successfully!" });
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
    }
}