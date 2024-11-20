using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace PayrollAPI.Migrations.HRMDBConnectMigrations
{
    /// <inheritdoc />
    public partial class change_approvl_workflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmpApprovals_Supervisor_approverIdid",
                table: "EmpApprovals");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpApprovals_WorkflowTypes_approvalLevelsid",
                table: "EmpApprovals");

            migrationBuilder.DropIndex(
                name: "IX_EmpApprovals_approvalLevelsid",
                table: "EmpApprovals");

            migrationBuilder.DropIndex(
                name: "IX_EmpApprovals_approverIdid",
                table: "EmpApprovals");

            migrationBuilder.DropColumn(
                name: "approvalLevelsid",
                table: "EmpApprovals");

            migrationBuilder.DropColumn(
                name: "approverIdid",
                table: "EmpApprovals");

            migrationBuilder.AddColumn<int>(
                name: "empApprovalWorkflowid",
                table: "EmpApprovals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EmpApprovalWorkflow",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    approvalLevelsid = table.Column<int>(type: "int", nullable: true),
                    approverIdid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpApprovalWorkflow", x => x.id);
                    table.ForeignKey(
                        name: "FK_EmpApprovalWorkflow_Supervisor_approverIdid",
                        column: x => x.approverIdid,
                        principalTable: "Supervisor",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_EmpApprovalWorkflow_WorkflowTypes_approvalLevelsid",
                        column: x => x.approvalLevelsid,
                        principalTable: "WorkflowTypes",
                        principalColumn: "id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_EmpApprovals_empApprovalWorkflowid",
                table: "EmpApprovals",
                column: "empApprovalWorkflowid");

            migrationBuilder.CreateIndex(
                name: "IX_EmpApprovalWorkflow_approvalLevelsid",
                table: "EmpApprovalWorkflow",
                column: "approvalLevelsid");

            migrationBuilder.CreateIndex(
                name: "IX_EmpApprovalWorkflow_approverIdid",
                table: "EmpApprovalWorkflow",
                column: "approverIdid");

            migrationBuilder.AddForeignKey(
                name: "FK_EmpApprovals_EmpApprovalWorkflow_empApprovalWorkflowid",
                table: "EmpApprovals",
                column: "empApprovalWorkflowid",
                principalTable: "EmpApprovalWorkflow",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmpApprovals_EmpApprovalWorkflow_empApprovalWorkflowid",
                table: "EmpApprovals");

            migrationBuilder.DropTable(
                name: "EmpApprovalWorkflow");

            migrationBuilder.DropIndex(
                name: "IX_EmpApprovals_empApprovalWorkflowid",
                table: "EmpApprovals");

            migrationBuilder.DropColumn(
                name: "empApprovalWorkflowid",
                table: "EmpApprovals");

            migrationBuilder.AddColumn<int>(
                name: "approvalLevelsid",
                table: "EmpApprovals",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "approverIdid",
                table: "EmpApprovals",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmpApprovals_approvalLevelsid",
                table: "EmpApprovals",
                column: "approvalLevelsid");

            migrationBuilder.CreateIndex(
                name: "IX_EmpApprovals_approverIdid",
                table: "EmpApprovals",
                column: "approverIdid");

            migrationBuilder.AddForeignKey(
                name: "FK_EmpApprovals_Supervisor_approverIdid",
                table: "EmpApprovals",
                column: "approverIdid",
                principalTable: "Supervisor",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmpApprovals_WorkflowTypes_approvalLevelsid",
                table: "EmpApprovals",
                column: "approvalLevelsid",
                principalTable: "WorkflowTypes",
                principalColumn: "id");
        }
    }
}
