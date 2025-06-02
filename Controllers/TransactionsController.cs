using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineGallery.Data;
using OnlineGallery.Helper;
using OnlineGallery.Models;
using Microsoft.EntityFrameworkCore;
using OnlineGallery.Helper;

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

            var artwork = _context.Artworks.Find(id);
            if (artwork == null)
                return NotFound();

            if (artwork.ArtistId == buyerId)
                return BadRequest("Cannot buy your own artwork.");

            if (artwork.Status != ArtworkStatus.Available)
                return BadRequest("This artwork is no longer available.");

            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole == Role.Admin.ToString())
                return BadRequest("Admins are not allowed to perform purchases.");

            // extra elegxos, o banned user den mporei na kanei oute login
            if (UserHelper.IsUserBanned(_context, buyerId.Value))
                return BadRequest("Banned users cannot make purchases.");

            if (UserHelper.IsUserBanned(_context, artwork.ArtistId))
                return BadRequest("Cannot purchase artwork from a banned user.");

            // create pending transaction
            var tx = new TransactionsModel
            {
                ArtworkId = id,
                BuyerId = buyerId.Value,
                Price = artwork.Price,
                PurchasedAt = TimeAthens.GetAthensTime(),
                Status = TransactionStatus.Pending
            };
            _context.Transactions.Add(tx);
            _context.SaveChanges();

            HttpContext.Session.SetBool("RedirectToPurchases", true);

            return RedirectToAction("Profile", "Users");
        }



        // 2) pending --> pay --> completed
        [HttpPost]
        public IActionResult Pay(int transactionId)
        {
            var buyerId = HttpContext.Session.GetInt32("UserId");
            if (buyerId == null)
                return RedirectToAction("Login", "Users");

            var tx = _context.Transactions
                .Include(t => t.Artwork)
                .FirstOrDefault(t => t.Id == transactionId && t.BuyerId == buyerId.Value);

            if (tx == null)
                return NotFound();

            if (tx.Status != TransactionStatus.Pending)
                return BadRequest("Transaction is not pending.");

            if (tx.Artwork.Status != ArtworkStatus.Available)
            {
                tx.Status = TransactionStatus.Cancelled;
                _context.SaveChanges();
                return BadRequest("This artwork is no longer available.");
            }

            tx.Status = TransactionStatus.Completed;
            tx.Artwork.Status = ArtworkStatus.Sold;

            _context.SaveChanges();

            UserRoleHelper.UpdateUserRole(_context, buyerId.Value); // helper

            return RedirectToAction("Profile", "Users", new { section = "purchases" });
        }

        [HttpPost]
        public IActionResult Cancel(int transactionId)
        {
            var buyerId = HttpContext.Session.GetInt32("UserId");
            if (buyerId == null)
                return RedirectToAction("Login", "Users");

            var tx = _context.Transactions
                .Include(t => t.Artwork)
                .FirstOrDefault(t => t.Id == transactionId && t.BuyerId == buyerId.Value);

            if (tx == null || tx.Status != (int)TransactionStatus.Pending)
                return NotFound();

            tx.Status = TransactionStatus.Cancelled;

            tx.Artwork.Status = ArtworkStatus.Available;

            _context.SaveChanges();
            return RedirectToAction("Profile", "Users", new { section = "purchases" });
        }



    }

}
