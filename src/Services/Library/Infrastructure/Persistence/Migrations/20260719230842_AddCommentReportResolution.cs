using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaForge.Library.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCommentReportResolution : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_resolved",
                table: "comment_reports",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "resolved_at",
                table: "comment_reports",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_resolved",
                table: "comment_reports");

            migrationBuilder.DropColumn(
                name: "resolved_at",
                table: "comment_reports");
        }
    }
}
