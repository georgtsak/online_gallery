namespace OnlineGallery.Models
{
    public class BanStatusModel
    {
        public User User { get; set; }
        public bool IsCurrentlyBanned { get; set; }
        public List<AdminModel> History { get; set; }

    }

}
