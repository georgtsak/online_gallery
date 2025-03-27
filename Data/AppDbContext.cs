using Microsoft.EntityFrameworkCore;
using OnlineGallery.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace OnlineGallery.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options)
			: base(options) { }

		public DbSet<User> Users { get; set; }
		public DbSet<ArtworkModel> Artworks { get; set; }
		public DbSet<TransactionModel> Transactions { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<TransactionModel>()
				.HasOne(t => t.Artwork)
				.WithMany()
				.HasForeignKey(t => t.ArtworkId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<TransactionModel>()
				.HasOne(t => t.Buyer)
				.WithMany()
				.HasForeignKey(t => t.BuyerId)
				.OnDelete(DeleteBehavior.NoAction);
		}


	}
}
