using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace PayrollAPI.Migrations
{
    /// <inheritdoc />
    public partial class manualmigration29102024 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isTaxableGross",
                table: "PayCode");

            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "Temp_Employee",
                type: "varchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "phoneNo",
                table: "Temp_Employee",
                type: "varchar(15)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "taxCategory",
                table: "Tax_Calculation",
                type: "varchar(2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "bankFileCreatedBy",
                table: "Payrun",
                type: "varchar(10)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "bankFileCreatedDate",
                table: "Payrun",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "bankFileCreatedTime",
                table: "Payrun",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "taxationType",
                table: "PayCode",
                type: "varchar(2)",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "EPFCOM",
                table: "GetSummaryDetails",
                type: "float",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "EPFEMP",
                table: "GetSummaryDetails",
                type: "float",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "ETF",
                table: "GetSummaryDetails",
                type: "float",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<decimal>(
                name: "grossAmount",
                table: "EPF_ETF",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "lumpSumGross",
                table: "EPF_ETF",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "netAmount",
                table: "EPF_ETF",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "Employee_Data",
                type: "varchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "phoneNo",
                table: "Employee_Data",
                type: "varchar(15)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BackgroudJobs",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    companyCode = table.Column<int>(type: "int", nullable: false),
                    period = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "varchar(50)", nullable: false),
                    createdBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    createdDate = table.Column<DateTime>(type: "datetime(6)", nullable: true, defaultValueSql: "(CURDATE())"),
                    createdTime = table.Column<DateTime>(type: "datetime(6)", nullable: true, defaultValueSql: "(CURTIME())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackgroudJobs", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PaySheet_Log",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    companyCode = table.Column<int>(type: "int", nullable: false),
                    epf = table.Column<string>(type: "varchar(6)", nullable: false),
                    period = table.Column<int>(type: "int", nullable: false),
                    isPaysheetUploaded = table.Column<bool>(type: "boolean(1)", nullable: false),
                    isSMSSend = table.Column<bool>(type: "boolean(1)", nullable: false),
                    paysheetID = table.Column<string>(type: "varchar(15)", nullable: true),
                    message = table.Column<string>(type: "varchar(300)", nullable: true),
                    changeBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    changeDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaySheet_Log", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Sys_Properties",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    companyCode = table.Column<int>(type: "int", nullable: false),
                    category_name = table.Column<string>(type: "varchar(50)", nullable: false),
                    variable_name = table.Column<string>(type: "varchar(50)", nullable: false),
                    variable_value = table.Column<string>(type: "varchar(500)", nullable: false),
                    createdBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    createdDate = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "(CURDATE())"),
                    createdTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "(CURTIME())"),
                    lastUpdateBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    lastUpdateDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    lastUpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sys_Properties", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BackgroudJobs");

            migrationBuilder.DropTable(
                name: "PaySheet_Log");

            migrationBuilder.DropTable(
                name: "Sys_Properties");

            migrationBuilder.DropColumn(
                name: "email",
                table: "Temp_Employee");

            migrationBuilder.DropColumn(
                name: "phoneNo",
                table: "Temp_Employee");

            migrationBuilder.DropColumn(
                name: "taxCategory",
                table: "Tax_Calculation");

            migrationBuilder.DropColumn(
                name: "bankFileCreatedBy",
                table: "Payrun");

            migrationBuilder.DropColumn(
                name: "bankFileCreatedDate",
                table: "Payrun");

            migrationBuilder.DropColumn(
                name: "bankFileCreatedTime",
                table: "Payrun");

            migrationBuilder.DropColumn(
                name: "taxationType",
                table: "PayCode");

            migrationBuilder.DropColumn(
                name: "EPFCOM",
                table: "GetSummaryDetails");

            migrationBuilder.DropColumn(
                name: "EPFEMP",
                table: "GetSummaryDetails");

            migrationBuilder.DropColumn(
                name: "ETF",
                table: "GetSummaryDetails");

            migrationBuilder.DropColumn(
                name: "grossAmount",
                table: "EPF_ETF");

            migrationBuilder.DropColumn(
                name: "lumpSumGross",
                table: "EPF_ETF");

            migrationBuilder.DropColumn(
                name: "netAmount",
                table: "EPF_ETF");

            migrationBuilder.DropColumn(
                name: "email",
                table: "Employee_Data");

            migrationBuilder.DropColumn(
                name: "phoneNo",
                table: "Employee_Data");

            migrationBuilder.AddColumn<bool>(
                name: "isTaxableGross",
                table: "PayCode",
                type: "boolean(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
