namespace OnlineGallery.Models
{
    public class ProfileModel
    {
        public User User { get; set; }
        public List<ArtworksModel> Artworks { get; set; }
        public List<ArtworksModel> AllArtworks { get; set; }
        public List<TransactionsModel> Purchases { get; set; }
        public List<TransactionsModel> Sales { get; set; }
        public List<BuyerModalModel> BuyerInfo { get; set; }
        public bool IsBanned { get; set; }
    }

}
