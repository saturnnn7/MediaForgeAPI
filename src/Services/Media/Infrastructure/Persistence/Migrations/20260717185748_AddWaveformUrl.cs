using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaForge.Media.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWaveformUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "waveform_url",
                table: "media_assets",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "waveform_url",
                table: "media_assets");
        }
    }
}
