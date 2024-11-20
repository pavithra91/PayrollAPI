using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollAPI.Migrations.HRMDBConnectMigrations
{
    /// <inheritdoc />
    public partial class change_approvl_workflow_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmpApprovalWorkflow_EmpApprovalWorkflow_empApprovalWorkflowid",
                table: "EmpApprovalWorkflow");

            migrationBuilder.DropIndex(
                name: "IX_EmpApprovalWorkflow_empApprovalWorkflowid",
                table: "EmpApprovalWorkflow");

            migrationBuilder.DropColumn(
                name: "empApprovalWorkflowid",
                table: "EmpApprovalWorkflow");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "empApprovalWorkflowid",
                table: "EmpApprovalWorkflow",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_EmpApprovalWorkflow_empApprovalWorkflowid",
                table: "EmpApprovalWorkflow",
                column: "empApprovalWorkflowid");

            migrationBuilder.AddForeignKey(
                name: "FK_EmpApprovalWorkflow_EmpApprovalWorkflow_empApprovalWorkflowid",
                table: "EmpApprovalWorkflow",
                column: "empApprovalWorkflowid",
                principalTable: "EmpApprovalWorkflow",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
