using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaForge.Catalog.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "work_requests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    requester_id = table.Column<Guid>(type: "uuid", nullable: false),
                    work_type = table.Column<string>(type: "text", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    author_names = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    cover_url = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    language = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    status = table.Column<string>(type: "text", nullable: false, defaultValue: "Pending"),
                    admin_note = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    resulting_work_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    reviewed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_requests", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_work_requests_requester_id",
                table: "work_requests",
                column: "requester_id");

            migrationBuilder.CreateIndex(
                name: "ix_work_requests_status",
                table: "work_requests",
                column: "status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "work_requests");
        }
    }
}
