using OnlineGallery.Helper;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineGallery.Models
{
    public class TransactionsModel
    {
        public int Id { get; set; }

        public int BuyerId { get; set; }
        [ForeignKey("BuyerId")]
        public User Buyer { get; set; }
        public int ArtworkId { get; set; }
        [ForeignKey("ArtworkId")]
        public ArtworksModel Artwork { get; set; }
        public decimal Price { get; set; }
        public DateTime PurchasedAt { get; set; } = TimeAthens.GetAthensTime();
        public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
    }

    public enum TransactionStatus
    {
        Pending = 0,
        Completed = 1,
        Cancelled = 2
    }
}
