using System.ComponentModel.DataAnnotations;
using System.Transactions;

namespace OnlineGallery.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string Salt { get; set; } // hashing kwdikou

        public string Role { get; set; } // artist or buyer

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<ArtworkModel> Artworks { get; set; } // artist
        public ICollection<TransactionModel> Transactions { get; set; } // buyer
    }


}
