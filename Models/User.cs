﻿using OnlineGallery.Helper;
using System.ComponentModel.DataAnnotations;
using System.Transactions;

namespace OnlineGallery.Models
{
	public class User
	{
		public int Id { get; set; }
		public string FullName { get; set; }

		[DataType(DataType.EmailAddress)]
		public string Email { get; set; }

		[DataType(DataType.Password)]
		public string Password { get; set; }
        public string Salt { get; set; } = ""; // hashing
        public DateTime CreatedAt { get; set; } = TimeAthens.GetAthensTime();
        public ICollection<ArtworksModel> Artworks { get; set; } = new List<ArtworksModel>(); // erga texnhs
		public ICollection<TransactionsModel> Transactions { get; set; } = new List<TransactionsModel>(); // agores

		public Role Role { get; set; } = Role.Visitor; // default
        public bool IsDeleted { get; set; } = false;
        public string? ProfileImgUrl { get; set; }
        public string ProfileImgUrlOrDefault
        {
            get
            {
                return string.IsNullOrEmpty(ProfileImgUrl)
                    ? "https://sofwlyfuejegiceqpgzw.supabase.co/storage/v1/object/public/profile-img/default.jpg"
                    : ProfileImgUrl;
            }
        }
    }

	public enum Role
	{
		Admin = 1,
		Visitor = 2,
		Buyer = 3,
		Artist = 4,
		BuyerAndArtist = 5 // exei agorasei + anarthsei toulaxiston 1 ergo texnhs
	}


}
