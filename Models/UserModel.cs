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

		public string Salt { get; set; } //hashing

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public ICollection<ArtworkModel> Artworks { get; set; } = new List<ArtworkModel>(); // erga texnhs
		public ICollection<TransactionModel> Transactions { get; set; } = new List<TransactionModel>(); // agores

		public string Role
		{
			get
			{
				bool isArtist = Artworks.Any();
				bool isBuyer = Transactions.Any();

				if (isArtist && isBuyer)
					return "Artist & Buyer";
				else if (isArtist)
					return "Artist";
				else if (isBuyer)
					return "Buyer";
				else
					return "Visitor"; // den exei kanei akomh kamia energeia
			}
		}
	}



}
