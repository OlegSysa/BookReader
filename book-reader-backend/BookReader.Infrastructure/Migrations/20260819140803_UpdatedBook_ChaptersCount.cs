using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookReader.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedBook_ChaptersCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChaptersCount",
                table: "books",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChaptersCount",
                table: "books");
        }
    }
}
