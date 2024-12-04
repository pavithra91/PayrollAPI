using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollAPI.Migrations.HRMDBConnectMigrations
{
    /// <inheritdoc />
    public partial class addFks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "employeeid",
                table: "EmpApprovals",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmpApprovals_employeeid",
                table: "EmpApprovals",
                column: "employeeid");

            migrationBuilder.AddForeignKey(
                name: "FK_EmpApprovals_Employee_employeeid",
                table: "EmpApprovals",
                column: "employeeid",
                principalTable: "Employee",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmpApprovals_Employee_employeeid",
                table: "EmpApprovals");

            migrationBuilder.DropIndex(
                name: "IX_EmpApprovals_employeeid",
                table: "EmpApprovals");

            migrationBuilder.DropColumn(
                name: "employeeid",
                table: "EmpApprovals");
        }
    }
}
