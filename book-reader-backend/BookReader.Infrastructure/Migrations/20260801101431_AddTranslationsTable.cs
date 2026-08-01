using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BookReader.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTranslationsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "translations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SourceLang = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    TargetLang = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    SourceWord = table.Column<string>(type: "text", nullable: false),
                    TranslatedWord = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_translations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Translations_Search",
                table: "translations",
                columns: new[] { "SourceLang", "TargetLang", "SourceWord" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "translations");
        }
    }
}
