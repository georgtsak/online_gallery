using OnlineGallery.Helper;

namespace OnlineGallery.Models
{
    public class TransactionsModel
    {
        public int Id { get; set; }
        public int BuyerId { get; set; }
        public User Buyer { get; set; }
        public int ArtworkId { get; set; }
        public ArtworksModel Artwork { get; set; }
        public decimal Price { get; set; }
        public DateTime PurchasedAt { get; set; } = TimeAthens.GetAthensTime();
        public TransactionStatus Status { get; set; } = TransactionStatus.Pending; // default: pending
    }

    public enum TransactionStatus
    {
        Pending = 0,
        Completed = 1,
        Cancelled = 2
    }
}
