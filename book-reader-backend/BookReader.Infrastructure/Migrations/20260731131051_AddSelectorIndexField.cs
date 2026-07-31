using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookReader.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSelectorIndexField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SelectorIndex",
                table: "chapters",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SelectorIndex",
                table: "chapters");
        }
    }
}
