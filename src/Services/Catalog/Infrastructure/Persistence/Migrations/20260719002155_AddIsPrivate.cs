using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaForge.Catalog.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIsPrivate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_private",
                table: "works",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_private",
                table: "parts",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_private",
                table: "works");

            migrationBuilder.DropColumn(
                name: "is_private",
                table: "parts");
        }
    }
}
