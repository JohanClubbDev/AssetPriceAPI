using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetPriceAPI.Migrations
{
    /// <inheritdoc />
    public partial class AdjustPriceColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValidFrom",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "ValidTo",
                table: "Prices");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ValidFrom",
                table: "Prices",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidTo",
                table: "Prices",
                type: "TEXT",
                nullable: true);
        }
    }
}
