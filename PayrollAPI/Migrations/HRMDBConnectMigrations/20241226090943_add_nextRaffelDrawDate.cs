using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollAPI.Migrations.HRMDBConnectMigrations
{
    /// <inheritdoc />
    public partial class add_nextRaffelDrawDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "raffleDrawOrder",
                table: "ReservationCategory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "bookingPriority",
                table: "Reservation",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "bookingStatus",
                table: "RaffleDraw",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "nextRaffelDrawDate",
                table: "Bungalow",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "raffleDrawOrder",
                table: "ReservationCategory");

            migrationBuilder.DropColumn(
                name: "bookingPriority",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "bookingStatus",
                table: "RaffleDraw");

            migrationBuilder.DropColumn(
                name: "nextRaffelDrawDate",
                table: "Bungalow");
        }
    }
}
