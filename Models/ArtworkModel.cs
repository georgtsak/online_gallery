namespace OnlineGallery.Models
{
    public class ArtworkModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public int ArtistId { get; set; }
        public User Artist { get; set; }
        public string Status { get; set; } = "Available"; // default timh
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
