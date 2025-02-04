using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoundPlayer.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Addfavoriteplaylistproperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FavoritePlaylistId",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_FavoritePlaylistId",
                table: "AspNetUsers",
                column: "FavoritePlaylistId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Playlists_FavoritePlaylistId",
                table: "AspNetUsers",
                column: "FavoritePlaylistId",
                principalTable: "Playlists",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Playlists_FavoritePlaylistId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_FavoritePlaylistId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FavoritePlaylistId",
                table: "AspNetUsers");
        }
    }
}
