namespace OnlineGallery.Models
{
    public class TransactionModel
    {
        public int Id { get; set; }
        public int BuyerId { get; set; }
        public User Buyer { get; set; }
        public int ArtworkId { get; set; }
        public ArtworksModel Artwork { get; set; }
        public decimal Price { get; set; }
        public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;
    }

}
