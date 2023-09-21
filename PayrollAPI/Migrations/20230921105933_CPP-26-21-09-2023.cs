using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollAPI.Migrations
{
    /// <inheritdoc />
    public partial class CPP2621092023 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "epf",
                table: "User",
                type: "varchar(6)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "paytype",
                table: "Temp_Payroll",
                type: "varchar(1)",
                nullable: true,
                oldClrType: typeof(ulong),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "epf",
                table: "Temp_Payroll",
                type: "varchar(6)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "epf",
                table: "Temp_Employee",
                type: "varchar(6)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "empName",
                table: "Temp_Employee",
                type: "varchar(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "empGrade",
                table: "Temp_Employee",
                type: "varchar(5)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "createdDate",
                table: "Temp_Employee",
                type: "datetime(6)",
                nullable: false,
                defaultValueSql: "CURDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<string>(
                name: "accountNo",
                table: "Temp_Employee",
                type: "varchar(15)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "epf",
                table: "Special_Tax_Emp",
                type: "varchar(6)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "paytype",
                table: "Payroll_Data",
                type: "varchar(1)",
                nullable: true,
                oldClrType: typeof(ulong),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "epf",
                table: "Payroll_Data",
                type: "varchar(6)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "epf",
                table: "EPF_ETF",
                type: "varchar(6)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "epf",
                table: "EmpSpecialRate",
                type: "varchar(6)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "epf",
                table: "Employee_Data",
                type: "varchar(6)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "epf",
                table: "User",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(6)");

            migrationBuilder.AlterColumn<ulong>(
                name: "paytype",
                table: "Temp_Payroll",
                type: "bit",
                nullable: false,
                defaultValue: 0ul,
                oldClrType: typeof(string),
                oldType: "varchar(1)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "epf",
                table: "Temp_Payroll",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(6)");

            migrationBuilder.AlterColumn<int>(
                name: "epf",
                table: "Temp_Employee",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(6)");

            migrationBuilder.AlterColumn<string>(
                name: "empName",
                table: "Temp_Employee",
                type: "varchar(500)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "empGrade",
                table: "Temp_Employee",
                type: "varchar(2)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(5)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "createdDate",
                table: "Temp_Employee",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValueSql: "CURDATE()");

            migrationBuilder.AlterColumn<int>(
                name: "accountNo",
                table: "Temp_Employee",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "varchar(15)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "epf",
                table: "Special_Tax_Emp",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(6)");

            migrationBuilder.AlterColumn<ulong>(
                name: "paytype",
                table: "Payroll_Data",
                type: "bit",
                nullable: false,
                defaultValue: 0ul,
                oldClrType: typeof(string),
                oldType: "varchar(1)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "epf",
                table: "Payroll_Data",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(6)");

            migrationBuilder.AlterColumn<int>(
                name: "epf",
                table: "EPF_ETF",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(6)");

            migrationBuilder.AlterColumn<int>(
                name: "epf",
                table: "EmpSpecialRate",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(6)");

            migrationBuilder.AlterColumn<int>(
                name: "epf",
                table: "Employee_Data",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(6)");
        }
    }
}
