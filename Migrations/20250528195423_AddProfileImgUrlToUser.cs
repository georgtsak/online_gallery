using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineGallery.Migrations
{
    /// <inheritdoc />
    public partial class AddProfileImgUrlToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfileImgUrl",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileImgUrl",
                table: "Users");
        }
    }
}
