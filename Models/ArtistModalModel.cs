namespace OnlineGallery.Models
{
    public class ArtistModalModel
    {
        public User Artist { get; set; }
        public List<ArtworksModel> Artworks { get; set; }
        public bool IsBanned { get; set; }
    }
}
