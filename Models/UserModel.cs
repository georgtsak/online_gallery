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

		public string Salt { get; set; } // hashing

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public ICollection<ArtworkModel> Artworks { get; set; } = new List<ArtworkModel>(); // erga texnhs
		public ICollection<TransactionModel> Transactions { get; set; } = new List<TransactionModel>(); // agores

		public Role Role { get; set; } = Role.Visitor; // default
	}

	public enum Role
	{
		Admin = 1,
		Visitor = 2,
		Buyer = 3,
		Artist = 4,
		BuyerAndArtist = 5
	}


}
