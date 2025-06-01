using OnlineGallery.Helper;
using System;


namespace OnlineGallery.Models
{
    public class AdminModel
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }  // FK

        public int AdminId { get; set; }
        public User Admin { get; set; }  // FK

        public ActionType ActionType { get; set; }
        public DateTime ActionDate { get; set; } = TimeAthens.GetAthensTime();
    }

    public enum ActionType
    {
        Ban = 1,
        Unban = 2
    }
}
