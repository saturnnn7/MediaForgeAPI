using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaForge.Catalog.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddExternalRatings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "external_ratings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    work_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source = table.Column<string>(type: "text", nullable: false),
                    external_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    score = table.Column<double>(type: "double precision", nullable: true),
                    review_count = table.Column<int>(type: "integer", nullable: true),
                    external_url = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_fetched_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_external_ratings", x => x.id);
                    table.ForeignKey(
                        name: "FK_external_ratings_works_work_id",
                        column: x => x.work_id,
                        principalTable: "works",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_external_ratings_work_id",
                table: "external_ratings",
                column: "work_id");

            migrationBuilder.CreateIndex(
                name: "ix_external_ratings_work_source",
                table: "external_ratings",
                columns: new[] { "work_id", "source" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "external_ratings");
        }
    }
}
