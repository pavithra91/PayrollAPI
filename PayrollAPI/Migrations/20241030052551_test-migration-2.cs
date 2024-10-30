using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollAPI.Migrations
{
    /// <inheritdoc />
    public partial class testmigration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "finishedTime",
                table: "BackgroudJobs",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "finishedTime",
                table: "BackgroudJobs");
        }
    }
}
