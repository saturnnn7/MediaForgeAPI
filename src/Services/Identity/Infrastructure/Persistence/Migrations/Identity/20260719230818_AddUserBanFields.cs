using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaForge.Identity.Infrastructure.Persistence.Migrations.Identity
{
    /// <inheritdoc />
    public partial class AddUserBanFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ban_reason",
                table: "users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_banned",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ban_reason",
                table: "users");

            migrationBuilder.DropColumn(
                name: "is_banned",
                table: "users");
        }
    }
}
