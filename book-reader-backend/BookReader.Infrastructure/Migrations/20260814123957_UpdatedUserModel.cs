using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookReader.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedUserModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_users_GoogleId",
                table: "users");

            migrationBuilder.RenameColumn(
                name: "GoogleId",
                table: "users",
                newName: "ExternalId");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_users_ExternalId",
                table: "users",
                column: "ExternalId",
                unique: true,
                filter: "\"ExternalId\" IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_User_PasswordOrExternalId",
                table: "users",
                sql: "\"PasswordHash\" IS NOT NULL OR \"ExternalId\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_users_ExternalId",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CK_User_PasswordOrExternalId",
                table: "users");

            migrationBuilder.RenameColumn(
                name: "ExternalId",
                table: "users",
                newName: "GoogleId");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "users",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_GoogleId",
                table: "users",
                column: "GoogleId",
                unique: true);
        }
    }
}
