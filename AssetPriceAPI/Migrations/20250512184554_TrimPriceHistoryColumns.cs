using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetPriceAPI.Migrations
{
    /// <inheritdoc />
    public partial class TrimPriceHistoryColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "PriceHistories");

            migrationBuilder.DropColumn(
                name: "OriginalPriceId",
                table: "PriceHistories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "PriceHistories",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "OriginalPriceId",
                table: "PriceHistories",
                type: "TEXT",
                nullable: true);
        }
    }
}
