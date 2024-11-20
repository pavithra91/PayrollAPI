using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollAPI.Migrations.HRMDBConnectMigrations
{
    /// <inheritdoc />
    public partial class change_approvl_workflow_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmpApprovals_EmpApprovalWorkflow_empApprovalWorkflowid",
                table: "EmpApprovals");

            migrationBuilder.DropIndex(
                name: "IX_EmpApprovals_empApprovalWorkflowid",
                table: "EmpApprovals");

            migrationBuilder.DropColumn(
                name: "empApprovalWorkflowid",
                table: "EmpApprovals");

            migrationBuilder.AddColumn<int>(
                name: "empApprovalWorkflowid",
                table: "EmpApprovalWorkflow",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EmpApprovalsid",
                table: "EmpApprovals",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmpApprovalWorkflow_empApprovalWorkflowid",
                table: "EmpApprovalWorkflow",
                column: "empApprovalWorkflowid");

            migrationBuilder.CreateIndex(
                name: "IX_EmpApprovals_EmpApprovalsid",
                table: "EmpApprovals",
                column: "EmpApprovalsid");

            migrationBuilder.AddForeignKey(
                name: "FK_EmpApprovals_EmpApprovals_EmpApprovalsid",
                table: "EmpApprovals",
                column: "EmpApprovalsid",
                principalTable: "EmpApprovals",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmpApprovalWorkflow_EmpApprovalWorkflow_empApprovalWorkflowid",
                table: "EmpApprovalWorkflow",
                column: "empApprovalWorkflowid",
                principalTable: "EmpApprovalWorkflow",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmpApprovals_EmpApprovals_EmpApprovalsid",
                table: "EmpApprovals");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpApprovalWorkflow_EmpApprovalWorkflow_empApprovalWorkflowid",
                table: "EmpApprovalWorkflow");

            migrationBuilder.DropIndex(
                name: "IX_EmpApprovalWorkflow_empApprovalWorkflowid",
                table: "EmpApprovalWorkflow");

            migrationBuilder.DropIndex(
                name: "IX_EmpApprovals_EmpApprovalsid",
                table: "EmpApprovals");

            migrationBuilder.DropColumn(
                name: "empApprovalWorkflowid",
                table: "EmpApprovalWorkflow");

            migrationBuilder.DropColumn(
                name: "EmpApprovalsid",
                table: "EmpApprovals");

            migrationBuilder.AddColumn<int>(
                name: "empApprovalWorkflowid",
                table: "EmpApprovals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_EmpApprovals_empApprovalWorkflowid",
                table: "EmpApprovals",
                column: "empApprovalWorkflowid");

            migrationBuilder.AddForeignKey(
                name: "FK_EmpApprovals_EmpApprovalWorkflow_empApprovalWorkflowid",
                table: "EmpApprovals",
                column: "empApprovalWorkflowid",
                principalTable: "EmpApprovalWorkflow",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
