namespace OnlineGallery.Models
{
    public class CreateArtworkRequest
    {
        public IFormFile Image { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int ArtistId { get; set; }
    }

}