using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollAPI.Migrations
{
    /// <inheritdoc />
    public partial class _04092023 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "epfConRate",
                table: "Temp_Payroll",
                type: "float",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "taxConRate",
                table: "Temp_Payroll",
                type: "float",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "epfConRate",
                table: "Payroll_Data",
                type: "float",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<decimal>(
                name: "epfContribution",
                table: "Payroll_Data",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<float>(
                name: "taxConRate",
                table: "Payroll_Data",
                type: "float",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<decimal>(
                name: "taxContribution",
                table: "Payroll_Data",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "epfConRate",
                table: "Temp_Payroll");

            migrationBuilder.DropColumn(
                name: "taxConRate",
                table: "Temp_Payroll");

            migrationBuilder.DropColumn(
                name: "epfConRate",
                table: "Payroll_Data");

            migrationBuilder.DropColumn(
                name: "epfContribution",
                table: "Payroll_Data");

            migrationBuilder.DropColumn(
                name: "taxConRate",
                table: "Payroll_Data");

            migrationBuilder.DropColumn(
                name: "taxContribution",
                table: "Payroll_Data");
        }
    }
}
