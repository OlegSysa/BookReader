using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookReader.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCorrelationIdToOutboxMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CorrelationId",
                table: "OutboxMessages",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrelationId",
                table: "OutboxMessages");
        }
    }
}
