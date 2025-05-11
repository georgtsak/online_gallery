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
		public DbSet<ArtworksModel> Artworks { get; set; }
		public DbSet<TransactionsModel> Transactions { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<TransactionsModel>()
				.HasOne(t => t.Artwork)
				.WithMany()
				.HasForeignKey(t => t.ArtworkId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<TransactionsModel>()
				.HasOne(t => t.Buyer)
				.WithMany()
				.HasForeignKey(t => t.BuyerId)
				.OnDelete(DeleteBehavior.NoAction);

			// agnoei tous deleted users kai ta artworks pou eixan anevasei
            modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
            modelBuilder.Entity<ArtworksModel>().HasQueryFilter(a => !a.Artist.IsDeleted);


        }


    }
}
