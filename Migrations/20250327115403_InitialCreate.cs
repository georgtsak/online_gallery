using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineGallery.Migrations
{
	public partial class InitialCreate : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "Users",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
					Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
					Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
					Salt = table.Column<string>(type: "nvarchar(max)", nullable: false),
					CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
					Role = table.Column<int>(type: "int", nullable: false, defaultValue: 2) // default: Visitor
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Users", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "Artworks",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
					Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
					Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
					ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
					ArtistId = table.Column<int>(type: "int", nullable: false),
					Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
					CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Artworks", x => x.Id);
					table.ForeignKey(
						name: "FK_Artworks_Users_ArtistId",
						column: x => x.ArtistId,
						principalTable: "Users",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "Transactions",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					BuyerId = table.Column<int>(type: "int", nullable: false),
					ArtworkId = table.Column<int>(type: "int", nullable: false),
					Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
					PurchasedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
					UserModelId = table.Column<int>(type: "int", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Transactions", x => x.Id);
					table.ForeignKey(
						name: "FK_Transactions_Artworks_ArtworkId",
						column: x => x.ArtworkId,
						principalTable: "Artworks",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_Transactions_Users_BuyerId",
						column: x => x.BuyerId,
						principalTable: "Users",
						principalColumn: "Id");
					table.ForeignKey(
						name: "FK_Transactions_Users_UserModelId",
						column: x => x.UserModelId,
						principalTable: "Users",
						principalColumn: "Id");
				});

			migrationBuilder.CreateIndex(
				name: "IX_Artworks_ArtistId",
				table: "Artworks",
				column: "ArtistId");

			migrationBuilder.CreateIndex(
				name: "IX_Transactions_ArtworkId",
				table: "Transactions",
				column: "ArtworkId");

			migrationBuilder.CreateIndex(
				name: "IX_Transactions_BuyerId",
				table: "Transactions",
				column: "BuyerId");

			migrationBuilder.CreateIndex(
				name: "IX_Transactions_UserModelId",
				table: "Transactions",
				column: "UserModelId");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(name: "Transactions");
			migrationBuilder.DropTable(name: "Artworks");
			migrationBuilder.DropTable(name: "Users");
		}
	}
}
