using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookReader.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Rename_Bookmark_Field : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Bokkmark",
                table: "books",
                newName: "Bookmark");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Bookmark",
                table: "books",
                newName: "Bokkmark");
        }
    }
}
