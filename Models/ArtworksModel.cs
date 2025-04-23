using OnlineGallery.Helper;
using Supabase.Gotrue;

namespace OnlineGallery.Models
{
    public class ArtworksModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public int ArtistId { get; set; }
        public User Artist { get; set; }
        public ArtworkStatus Status { get; set; } = ArtworkStatus.Available;
        public DateTime CreatedAt { get; set; } = TimeAthens.GetAthensTime();
    }
    public enum ArtworkStatus
    {
        Available = 1,
        Sold = 0       
    }


}
