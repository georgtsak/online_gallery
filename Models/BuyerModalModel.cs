namespace OnlineGallery.Models
{
    public class BuyerModalModel
    {
        public User Buyer { get; set; }
        public int TotalPurchases { get; set; }
        public decimal TotalSpent { get; set; }
        public TransactionsModel MostExpensivePurchase { get; set; }
        public bool IsBanned { get; set; }
    }

}
