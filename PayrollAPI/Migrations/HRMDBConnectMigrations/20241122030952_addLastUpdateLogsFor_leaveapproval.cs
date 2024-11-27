using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollAPI.Migrations.HRMDBConnectMigrations
{
    /// <inheritdoc />
    public partial class addLastUpdateLogsFor_leaveapproval : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "lastUpdateBy",
                table: "LeaveApproval",
                type: "varchar(10)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "lastUpdateDate",
                table: "LeaveApproval",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "lastUpdateTime",
                table: "LeaveApproval",
                type: "time(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "lastUpdateBy",
                table: "LeaveApproval");

            migrationBuilder.DropColumn(
                name: "lastUpdateDate",
                table: "LeaveApproval");

            migrationBuilder.DropColumn(
                name: "lastUpdateTime",
                table: "LeaveApproval");
        }
    }
}
