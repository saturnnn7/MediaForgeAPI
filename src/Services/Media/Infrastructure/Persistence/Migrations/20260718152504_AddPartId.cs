using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaForge.Media.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPartId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "part_id",
                table: "media_assets",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "part_id",
                table: "media_assets");
        }
    }
}
