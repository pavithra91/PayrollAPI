using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollAPI.Migrations.HRMDBConnectMigrations
{
    /// <inheritdoc />
    public partial class addEmpGrades_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employee_EmployeeGrade_EmployeeGradeid",
                table: "Employee");

            migrationBuilder.DropForeignKey(
                name: "FK_Employee_Employee_empGradeid",
                table: "Employee");

            migrationBuilder.DropIndex(
                name: "IX_Employee_EmployeeGradeid",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "EmployeeGradeid",
                table: "Employee");

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_EmployeeGrade_empGradeid",
                table: "Employee",
                column: "empGradeid",
                principalTable: "EmployeeGrade",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employee_EmployeeGrade_empGradeid",
                table: "Employee");

            migrationBuilder.AddColumn<int>(
                name: "EmployeeGradeid",
                table: "Employee",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employee_EmployeeGradeid",
                table: "Employee",
                column: "EmployeeGradeid");

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_EmployeeGrade_EmployeeGradeid",
                table: "Employee",
                column: "EmployeeGradeid",
                principalTable: "EmployeeGrade",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_Employee_empGradeid",
                table: "Employee",
                column: "empGradeid",
                principalTable: "Employee",
                principalColumn: "id");
        }
    }
}
