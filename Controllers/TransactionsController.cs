using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineGallery.Data;
using OnlineGallery.Helper;
using OnlineGallery.Models;
namespace OnlineGallery.Controllers
{
    public class TransactionController : Controller
    {
        private readonly AppDbContext _context;

        public TransactionController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Buy(int id)
        {
            var buyerId = HttpContext.Session.GetInt32("UserId");
            if (buyerId == null)
                return RedirectToAction("Login", "Users");

            var artwork = _context.Artworks.FirstOrDefault(a => a.Id == id);
            if (artwork == null)
                return NotFound("Artwork not found.");

            if (artwork.ArtistId == buyerId)
                return BadRequest("You cannot purchase your own artwork.");

            var transaction = new TransactionsModel
            {
                ArtworkId = id,
                BuyerId = buyerId.Value,
                Price = artwork.Price,
                PurchasedAt = TimeAthens.GetAthensTime(),
                Status = (int)TransactionStatus.Pending
            };

            _context.Transactions.Add(transaction);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Transaction completed successfully!";
            return RedirectToAction("Index", "Artworks");
        }
    }
}
