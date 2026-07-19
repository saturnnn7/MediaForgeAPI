using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaForge.Catalog.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEditionEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "work_id",
                table: "parts",
                newName: "edition_id");

            migrationBuilder.RenameIndex(
                name: "ix_parts_work_order",
                table: "parts",
                newName: "ix_parts_edition_order");

            migrationBuilder.RenameIndex(
                name: "ix_parts_work_id",
                table: "parts",
                newName: "ix_parts_edition_id");

            migrationBuilder.CreateTable(
                name: "editions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    work_id = table.Column<Guid>(type: "uuid", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: false),
                    narrator_team_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    cover_url = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    language = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValue: "en"),
                    is_default = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_editions", x => x.id);
                    table.ForeignKey(
                        name: "FK_editions_works_work_id",
                        column: x => x.work_id,
                        principalTable: "works",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_editions_work_default",
                table: "editions",
                columns: new[] { "work_id", "is_default" });

            migrationBuilder.CreateIndex(
                name: "ix_editions_work_id",
                table: "editions",
                column: "work_id");

            // Data migration: create a default edition per existing work, then repoint parts
            // (whose "edition_id" column still holds the pre-rename work_id value) at it.
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS pgcrypto;");

            migrationBuilder.Sql(
                """
                INSERT INTO editions (id, work_id, creator_id, narrator_team_name, language, is_default, created_at)
                SELECT gen_random_uuid(), id, '00000000-0000-0000-0000-000000000000', 'Default', 'en', true, NOW()
                FROM works;
                """);

            migrationBuilder.Sql(
                """
                UPDATE parts
                SET edition_id = e.id
                FROM editions e
                WHERE e.work_id = parts.edition_id;
                """);

            migrationBuilder.AddForeignKey(
                name: "FK_parts_editions_edition_id",
                table: "parts",
                column: "edition_id",
                principalTable: "editions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_parts_editions_edition_id",
                table: "parts");

            migrationBuilder.DropTable(
                name: "editions");

            migrationBuilder.RenameColumn(
                name: "edition_id",
                table: "parts",
                newName: "work_id");

            migrationBuilder.RenameIndex(
                name: "ix_parts_edition_order",
                table: "parts",
                newName: "ix_parts_work_order");

            migrationBuilder.RenameIndex(
                name: "ix_parts_edition_id",
                table: "parts",
                newName: "ix_parts_work_id");
        }
    }
}
