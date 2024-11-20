using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollAPI.Migrations.HRMDBConnectMigrations
{
    /// <inheritdoc />
    public partial class change_approvl_workflow_4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "empApprovalIdid",
                table: "EmpApprovalWorkflow",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_EmpApprovalWorkflow_empApprovalIdid",
                table: "EmpApprovalWorkflow",
                column: "empApprovalIdid");

            migrationBuilder.AddForeignKey(
                name: "FK_EmpApprovalWorkflow_EmpApprovals_empApprovalIdid",
                table: "EmpApprovalWorkflow",
                column: "empApprovalIdid",
                principalTable: "EmpApprovals",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmpApprovalWorkflow_EmpApprovals_empApprovalIdid",
                table: "EmpApprovalWorkflow");

            migrationBuilder.DropIndex(
                name: "IX_EmpApprovalWorkflow_empApprovalIdid",
                table: "EmpApprovalWorkflow");

            migrationBuilder.DropColumn(
                name: "empApprovalIdid",
                table: "EmpApprovalWorkflow");
        }
    }
}
