using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoundPlayer.DAL.Migrations
{
    /// <inheritdoc />
    public partial class _1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Tracks",
                newName: "Name");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Duration",
                table: "Tracks",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Tracks");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Tracks",
                newName: "Title");
        }
    }
}
