using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollAPI.Migrations.HRMDBConnectMigrations
{
    /// <inheritdoc />
    public partial class addFks_advancepayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "epf",
                table: "AdvancePayment");

            migrationBuilder.AddColumn<int>(
                name: "employeeid",
                table: "AdvancePayment",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdvancePayment_employeeid",
                table: "AdvancePayment",
                column: "employeeid");

            migrationBuilder.AddForeignKey(
                name: "FK_AdvancePayment_Employee_employeeid",
                table: "AdvancePayment",
                column: "employeeid",
                principalTable: "Employee",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdvancePayment_Employee_employeeid",
                table: "AdvancePayment");

            migrationBuilder.DropIndex(
                name: "IX_AdvancePayment_employeeid",
                table: "AdvancePayment");

            migrationBuilder.DropColumn(
                name: "employeeid",
                table: "AdvancePayment");

            migrationBuilder.AddColumn<string>(
                name: "epf",
                table: "AdvancePayment",
                type: "longtext",
                nullable: false);
        }
    }
}
