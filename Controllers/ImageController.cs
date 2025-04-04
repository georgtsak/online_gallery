using Microsoft.AspNetCore.Mvc;
using OnlineGallery.Data;
using OnlineGallery.Models;

namespace OnlineGallery.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> UploadImage([FromForm] CreateImageRequest request,
        [FromServices] Supabase.Client client)
        {
            if (client == null)
            {
                return StatusCode(500, "Supabase client not injected!");
            }

            Console.WriteLine("Uploading image...");
            using var memoryStream = new MemoryStream();
            await request.Image.CopyToAsync(memoryStream);
            var imageBytes = memoryStream.ToArray();

            var bucket = client.Storage.From("artworks");
            var fileName = $"{Guid.NewGuid()}_{request.Image.FileName}";
            await bucket.Upload(imageBytes, fileName);

            var publicUrl = bucket.GetPublicUrl(fileName);

            return Ok(new { Url = publicUrl });
        }


    }
}
