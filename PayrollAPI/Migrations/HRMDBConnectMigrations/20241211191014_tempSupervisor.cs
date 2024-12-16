using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollAPI.Migrations.HRMDBConnectMigrations
{
    /// <inheritdoc />
    public partial class tempSupervisor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "expireDate",
                table: "Supervisor",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isTempSupervisor",
                table: "Supervisor",
                type: "tinyint(1)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "expireDate",
                table: "Supervisor");

            migrationBuilder.DropColumn(
                name: "isTempSupervisor",
                table: "Supervisor");
        }
    }
}
