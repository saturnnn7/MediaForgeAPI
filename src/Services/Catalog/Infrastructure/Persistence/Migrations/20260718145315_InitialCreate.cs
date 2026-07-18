using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaForge.Catalog.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "genres",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_genres", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "parts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    work_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    order_major = table.Column<int>(type: "integer", nullable: false),
                    order_minor = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    part_type = table.Column<string>(type: "text", nullable: false),
                    cover_url = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    duration_seconds = table.Column<double>(type: "double precision", nullable: true),
                    is_published = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "persons",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    bio = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    photo_url = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_persons", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "series",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    channel_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    cover_url = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    is_completed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_series", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "works",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    channel_id = table.Column<Guid>(type: "uuid", nullable: false),
                    series_id = table.Column<Guid>(type: "uuid", nullable: true),
                    work_type = table.Column<string>(type: "text", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    cover_url = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    language = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    published_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_published = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_works", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "part_assets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    part_id = table.Column<Guid>(type: "uuid", nullable: false),
                    media_asset_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sequence_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_part_assets", x => x.id);
                    table.ForeignKey(
                        name: "FK_part_assets_parts_part_id",
                        column: x => x.part_id,
                        principalTable: "parts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "part_chapters",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    part_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    start_time_seconds = table.Column<double>(type: "double precision", nullable: false),
                    end_time_seconds = table.Column<double>(type: "double precision", nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_part_chapters", x => x.id);
                    table.ForeignKey(
                        name: "FK_part_chapters_parts_part_id",
                        column: x => x.part_id,
                        principalTable: "parts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "work_contributors",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    work_id = table.Column<Guid>(type: "uuid", nullable: false),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role = table.Column<string>(type: "text", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_contributors", x => x.id);
                    table.ForeignKey(
                        name: "FK_work_contributors_works_work_id",
                        column: x => x.work_id,
                        principalTable: "works",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "work_genres",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    work_id = table.Column<Guid>(type: "uuid", nullable: false),
                    genre_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_genres", x => x.id);
                    table.ForeignKey(
                        name: "FK_work_genres_works_work_id",
                        column: x => x.work_id,
                        principalTable: "works",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_genres_slug",
                table: "genres",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_part_assets_part_id",
                table: "part_assets",
                column: "part_id");

            migrationBuilder.CreateIndex(
                name: "ix_part_assets_part_media_asset",
                table: "part_assets",
                columns: new[] { "part_id", "media_asset_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_part_chapters_part_id",
                table: "part_chapters",
                column: "part_id");

            migrationBuilder.CreateIndex(
                name: "ix_parts_work_id",
                table: "parts",
                column: "work_id");

            migrationBuilder.CreateIndex(
                name: "ix_parts_work_order",
                table: "parts",
                columns: new[] { "work_id", "order_major", "order_minor" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_persons_name",
                table: "persons",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_series_channel_id",
                table: "series",
                column: "channel_id");

            migrationBuilder.CreateIndex(
                name: "ix_work_contributors_person_id",
                table: "work_contributors",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "ix_work_contributors_work_person_role",
                table: "work_contributors",
                columns: new[] { "work_id", "person_id", "role" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_work_genres_work_genre",
                table: "work_genres",
                columns: new[] { "work_id", "genre_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_works_channel_id",
                table: "works",
                column: "channel_id");

            migrationBuilder.CreateIndex(
                name: "ix_works_is_published",
                table: "works",
                column: "is_published");

            migrationBuilder.CreateIndex(
                name: "ix_works_series_id",
                table: "works",
                column: "series_id");

            migrationBuilder.CreateIndex(
                name: "ix_works_work_type",
                table: "works",
                column: "work_type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "genres");

            migrationBuilder.DropTable(
                name: "part_assets");

            migrationBuilder.DropTable(
                name: "part_chapters");

            migrationBuilder.DropTable(
                name: "persons");

            migrationBuilder.DropTable(
                name: "series");

            migrationBuilder.DropTable(
                name: "work_contributors");

            migrationBuilder.DropTable(
                name: "work_genres");

            migrationBuilder.DropTable(
                name: "parts");

            migrationBuilder.DropTable(
                name: "works");
        }
    }
}
