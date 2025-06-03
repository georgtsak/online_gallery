using System.Linq;
using OnlineGallery.Data;
using OnlineGallery.Models;

namespace OnlineGallery.Helper
{
    public static class UserHelper
    {
        public static bool IsUserBanned(AppDbContext context, int userId)
        {
            if (context == null)
                return false;

            var lastAction = context.AdminActions
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.ActionDate)
                .FirstOrDefault();

            return lastAction != null && lastAction.ActionType == ActionType.Ban;
        }
    }
}
