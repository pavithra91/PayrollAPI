using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollAPI.Migrations
{
    /// <inheritdoc />
    public partial class _21122023_EPF_ETF_TOT_Table_Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "grade",
                table: "EPF_ETF",
                type: "varchar(5)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(2)");

            migrationBuilder.AddColumn<int>(
                name: "companyCode",
                table: "EPF_ETF",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "epfGross",
                table: "EPF_ETF",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "location",
                table: "EPF_ETF",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "lumpsumTax",
                table: "EPF_ETF",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "tax",
                table: "EPF_ETF",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "taxableGross",
                table: "EPF_ETF",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "companyCode",
                table: "EPF_ETF");

            migrationBuilder.DropColumn(
                name: "epfGross",
                table: "EPF_ETF");

            migrationBuilder.DropColumn(
                name: "location",
                table: "EPF_ETF");

            migrationBuilder.DropColumn(
                name: "lumpsumTax",
                table: "EPF_ETF");

            migrationBuilder.DropColumn(
                name: "tax",
                table: "EPF_ETF");

            migrationBuilder.DropColumn(
                name: "taxableGross",
                table: "EPF_ETF");

            migrationBuilder.AlterColumn<string>(
                name: "grade",
                table: "EPF_ETF",
                type: "varchar(2)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(5)",
                oldNullable: true);
        }
    }
}
