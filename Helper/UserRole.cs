using System.Linq;
using Microsoft.EntityFrameworkCore;
using OnlineGallery.Data;
using OnlineGallery.Models;

namespace OnlineGallery.Helper
{
    public static class UserRoleHelper
    {
        public static void UpdateUserRole(AppDbContext context, int userId)
        {
            var user = context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return;

            bool hasPurchases = context.Transactions
                .Any(t => t.BuyerId == userId && t.Status == TransactionStatus.Completed);

            bool hasArtworks = context.Artworks
                .Any(a => a.ArtistId == userId);

            Role newRole;

            if (hasPurchases && hasArtworks)
                newRole = Role.BuyerAndArtist;
            else if (hasPurchases)
                newRole = Role.Buyer;
            else if (hasArtworks)
                newRole = Role.Artist;
            else
                newRole = Role.Visitor;

            if (user.Role != newRole)
            {
                user.Role = newRole;
                context.SaveChanges();
            }
        }
    }
}
