namespace OnlineGallery.Models
{
    public class HomeModel
    {
        public List<ArtworksModel> RecentArtworks { get; set; }

        public List<TopArtists1> TopArtists { get; set; }

        public List<ArtworksModel> RecommendedArtworks { get; set; }

        public class TopArtists1
        {
            public User Artist { get; set; }                      
            public int SalesCount { get; set; }     
        }
        public List<ArtistModalModel> ArtistProfile { get; set; }
    }
}
