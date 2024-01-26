using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollAPI.Migrations
{
    /// <inheritdoc />
    public partial class CPP36220920231 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "createdDate",
                table: "User",
                type: "datetime",
                nullable: false,
                defaultValueSql: "(CURDATE())",
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AddColumn<DateTime>(
                name: "createdTime",
                table: "User",
                type: "datetime",
                nullable: false,
                defaultValueSql: "(CURTIME())");

            migrationBuilder.AddColumn<DateTime>(
                name: "lastUpdateTime",
                table: "User",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<DateTime>(
                name: "createdDate",
                table: "TotPayCode",
                type: "datetime",
                nullable: false,
                defaultValueSql: "(CURDATE())",
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AddColumn<DateTime>(
                name: "createdTime",
                table: "TotPayCode",
                type: "datetime",
                nullable: false,
                defaultValueSql: "(CURTIME())");

            migrationBuilder.AddColumn<DateTime>(
                name: "createdDate",
                table: "Temp_Payroll",
                type: "datetime",
                nullable: false,
                defaultValueSql: "(CURDATE())");

            migrationBuilder.AddColumn<DateTime>(
                name: "createdTime",
                table: "Temp_Payroll",
                type: "datetime",
                nullable: false,
                defaultValueSql: "(CURTIME())");

            migrationBuilder.AddColumn<DateTime>(
                name: "createdTime",
                table: "Temp_Employee",
                type: "datetime",
                nullable: false,
                defaultValueSql: "(CURTIME())");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createdDate",
                table: "Tax_Calculation",
                type: "datetime",
                nullable: false,
                defaultValueSql: "(CURDATE())",
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AddColumn<DateTime>(
                name: "createdTime",
                table: "Tax_Calculation",
                type: "datetime",
                nullable: false,
                defaultValueSql: "(CURTIME())");

            migrationBuilder.AddColumn<DateTime>(
                name: "lastUpdateTime",
                table: "Tax_Calculation",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<DateTime>(
                name: "createdDate",
                table: "Special_Tax_Emp",
                type: "datetime",
                nullable: false,
                defaultValueSql: "(CURDATE())",
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AddColumn<DateTime>(
                name: "createdTime",
                table: "Special_Tax_Emp",
                type: "datetime",
                nullable: false,
                defaultValueSql: "(CURTIME())");

            migrationBuilder.AddColumn<DateTime>(
                name: "lastUpdateTime",
                table: "Special_Tax_Emp",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "approvedTime",
                table: "Payrun",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "payrunTime",
                table: "Payrun",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<DateTime>(
                name: "createdDate",
                table: "PayCode",
                type: "datetime",
                nullable: false,
                defaultValueSql: "(CURDATE())",
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AddColumn<DateTime>(
                name: "createdTime",
                table: "PayCode",
                type: "datetime",
                nullable: false,
                defaultValueSql: "(CURTIME())");

            migrationBuilder.AddColumn<DateTime>(
                name: "lastUpdateTime",
                table: "PayCode",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "loginTime",
                table: "LoginInfo",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<DateTime>(
                name: "createdDate",
                table: "EPF_ETF",
                type: "datetime",
                nullable: false,
                defaultValueSql: "(CURDATE())",
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AddColumn<DateTime>(
                name: "createdTime",
                table: "EPF_ETF",
                type: "datetime",
                nullable: false,
                defaultValueSql: "(CURTIME())");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createdDate",
                table: "EmpSpecialRate",
                type: "datetime",
                nullable: false,
                defaultValueSql: "(CURDATE())",
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AddColumn<DateTime>(
                name: "createdTime",
                table: "EmpSpecialRate",
                type: "datetime",
                nullable: false,
                defaultValueSql: "(CURTIME())");

            migrationBuilder.AddColumn<DateTime>(
                name: "lastUpdateTime",
                table: "EmpSpecialRate",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<DateTime>(
                name: "createdDate",
                table: "Calculation",
                type: "datetime",
                nullable: false,
                defaultValueSql: "(CURDATE())",
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AddColumn<DateTime>(
                name: "createdTime",
                table: "Calculation",
                type: "datetime",
                nullable: false,
                defaultValueSql: "(CURTIME())");

            migrationBuilder.AddColumn<DateTime>(
                name: "lastUpdateTime",
                table: "Calculation",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "createdTime",
                table: "User");

            migrationBuilder.DropColumn(
                name: "lastUpdateTime",
                table: "User");

            migrationBuilder.DropColumn(
                name: "createdTime",
                table: "TotPayCode");

            migrationBuilder.DropColumn(
                name: "createdDate",
                table: "Temp_Payroll");

            migrationBuilder.DropColumn(
                name: "createdTime",
                table: "Temp_Payroll");

            migrationBuilder.DropColumn(
                name: "createdTime",
                table: "Temp_Employee");

            migrationBuilder.DropColumn(
                name: "createdTime",
                table: "Tax_Calculation");

            migrationBuilder.DropColumn(
                name: "lastUpdateTime",
                table: "Tax_Calculation");

            migrationBuilder.DropColumn(
                name: "createdTime",
                table: "Special_Tax_Emp");

            migrationBuilder.DropColumn(
                name: "lastUpdateTime",
                table: "Special_Tax_Emp");

            migrationBuilder.DropColumn(
                name: "approvedTime",
                table: "Payrun");

            migrationBuilder.DropColumn(
                name: "payrunTime",
                table: "Payrun");

            migrationBuilder.DropColumn(
                name: "createdTime",
                table: "PayCode");

            migrationBuilder.DropColumn(
                name: "lastUpdateTime",
                table: "PayCode");

            migrationBuilder.DropColumn(
                name: "loginTime",
                table: "LoginInfo");

            migrationBuilder.DropColumn(
                name: "createdTime",
                table: "EPF_ETF");

            migrationBuilder.DropColumn(
                name: "createdTime",
                table: "EmpSpecialRate");

            migrationBuilder.DropColumn(
                name: "lastUpdateTime",
                table: "EmpSpecialRate");

            migrationBuilder.DropColumn(
                name: "createdTime",
                table: "Calculation");

            migrationBuilder.DropColumn(
                name: "lastUpdateTime",
                table: "Calculation");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createdDate",
                table: "User",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValueSql: "(CURDATE())");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createdDate",
                table: "TotPayCode",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValueSql: "(CURDATE())");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createdDate",
                table: "Tax_Calculation",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValueSql: "(CURDATE())");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createdDate",
                table: "Special_Tax_Emp",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValueSql: "(CURDATE())");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createdDate",
                table: "PayCode",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValueSql: "(CURDATE())");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createdDate",
                table: "EPF_ETF",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValueSql: "(CURDATE())");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createdDate",
                table: "EmpSpecialRate",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValueSql: "(CURDATE())");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createdDate",
                table: "Calculation",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValueSql: "(CURDATE())");
        }
    }
}
