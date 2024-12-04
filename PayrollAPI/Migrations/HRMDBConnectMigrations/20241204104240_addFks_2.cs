using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollAPI.Migrations.HRMDBConnectMigrations
{
    /// <inheritdoc />
    public partial class addFks_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "workflowLevelid",
                table: "Employee",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employee_workflowLevelid",
                table: "Employee",
                column: "workflowLevelid");

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_WorkflowTypes_workflowLevelid",
                table: "Employee",
                column: "workflowLevelid",
                principalTable: "WorkflowTypes",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employee_WorkflowTypes_workflowLevelid",
                table: "Employee");

            migrationBuilder.DropIndex(
                name: "IX_Employee_workflowLevelid",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "workflowLevelid",
                table: "Employee");
        }
    }
}
