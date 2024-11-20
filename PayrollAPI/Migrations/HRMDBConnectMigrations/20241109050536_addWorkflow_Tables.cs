using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace PayrollAPI.Migrations.HRMDBConnectMigrations
{
    /// <inheritdoc />
    public partial class addWorkflow_Tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeLeave");

            migrationBuilder.DropColumn(
                name: "actionedDate",
                table: "LeaveRequest");

            migrationBuilder.DropColumn(
                name: "actionedTime",
                table: "LeaveRequest");

            migrationBuilder.DropColumn(
                name: "status",
                table: "LeaveRequest");

            migrationBuilder.AddColumn<string>(
                name: "actingDelegate",
                table: "LeaveRequest",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "actingDelegateApprovalStatus",
                table: "LeaveRequest",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "actingDelegateApprovedDate",
                table: "LeaveRequest",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "actingDelegateApprovedTime",
                table: "LeaveRequest",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "currentLevel",
                table: "LeaveRequest",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "finalStatus",
                table: "LeaveRequest",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "requestStatus",
                table: "LeaveRequest",
                type: "longtext",
                nullable: false);

            migrationBuilder.CreateTable(
                name: "Supervisor",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    userId = table.Column<string>(type: "longtext", nullable: false),
                    epf = table.Column<string>(type: "longtext", nullable: false),
                    createdBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    createdDate = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "(CURDATE())"),
                    createdTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "(CURTIME())"),
                    lastUpdateBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    lastUpdateDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    lastUpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supervisor", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "WorkflowTypes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    levelName = table.Column<string>(type: "longtext", nullable: false),
                    createdBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    createdDate = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "(CURDATE())"),
                    createdTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "(CURTIME())"),
                    lastUpdateBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    lastUpdateDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    lastUpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowTypes", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ApprovalWorkflow",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    approvalLevelsid = table.Column<int>(type: "int", nullable: true),
                    approverIdid = table.Column<int>(type: "int", nullable: true),
                    epf = table.Column<int>(type: "int", nullable: false),
                    level = table.Column<int>(type: "int", nullable: true),
                    createdBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    createdDate = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "(CURDATE())"),
                    createdTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "(CURTIME())"),
                    lastUpdateBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    lastUpdateDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    lastUpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalWorkflow", x => x.id);
                    table.ForeignKey(
                        name: "FK_ApprovalWorkflow_Supervisor_approverIdid",
                        column: x => x.approverIdid,
                        principalTable: "Supervisor",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_ApprovalWorkflow_WorkflowTypes_approvalLevelsid",
                        column: x => x.approvalLevelsid,
                        principalTable: "WorkflowTypes",
                        principalColumn: "id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LeaveApproval",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    epf = table.Column<string>(type: "longtext", nullable: false),
                    requestIdleaveRequestId = table.Column<int>(type: "int", nullable: false),
                    levelid = table.Column<int>(type: "int", nullable: false),
                    approver_idid = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "longtext", nullable: false),
                    comments = table.Column<string>(type: "varchar(100)", nullable: true),
                    createdDate = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "(CURDATE())"),
                    createdTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "(CURTIME())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveApproval", x => x.id);
                    table.ForeignKey(
                        name: "FK_LeaveApproval_LeaveRequest_requestIdleaveRequestId",
                        column: x => x.requestIdleaveRequestId,
                        principalTable: "LeaveRequest",
                        principalColumn: "leaveRequestId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeaveApproval_Supervisor_approver_idid",
                        column: x => x.approver_idid,
                        principalTable: "Supervisor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeaveApproval_WorkflowTypes_levelid",
                        column: x => x.levelid,
                        principalTable: "WorkflowTypes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalWorkflow_approvalLevelsid",
                table: "ApprovalWorkflow",
                column: "approvalLevelsid");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalWorkflow_approverIdid",
                table: "ApprovalWorkflow",
                column: "approverIdid");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveApproval_approver_idid",
                table: "LeaveApproval",
                column: "approver_idid");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveApproval_levelid",
                table: "LeaveApproval",
                column: "levelid");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveApproval_requestIdleaveRequestId",
                table: "LeaveApproval",
                column: "requestIdleaveRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApprovalWorkflow");

            migrationBuilder.DropTable(
                name: "LeaveApproval");

            migrationBuilder.DropTable(
                name: "Supervisor");

            migrationBuilder.DropTable(
                name: "WorkflowTypes");

            migrationBuilder.DropColumn(
                name: "actingDelegate",
                table: "LeaveRequest");

            migrationBuilder.DropColumn(
                name: "actingDelegateApprovalStatus",
                table: "LeaveRequest");

            migrationBuilder.DropColumn(
                name: "actingDelegateApprovedDate",
                table: "LeaveRequest");

            migrationBuilder.DropColumn(
                name: "actingDelegateApprovedTime",
                table: "LeaveRequest");

            migrationBuilder.DropColumn(
                name: "currentLevel",
                table: "LeaveRequest");

            migrationBuilder.DropColumn(
                name: "finalStatus",
                table: "LeaveRequest");

            migrationBuilder.DropColumn(
                name: "requestStatus",
                table: "LeaveRequest");

            migrationBuilder.AddColumn<DateTime>(
                name: "actionedDate",
                table: "LeaveRequest",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "actionedTime",
                table: "LeaveRequest",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "LeaveRequest",
                type: "varchar(10)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "EmployeeLeave",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    companyCode = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeLeave", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");
        }
    }
}
