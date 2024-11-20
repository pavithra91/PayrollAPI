using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace PayrollAPI.Migrations.HRMDBConnectMigrations
{
    /// <inheritdoc />
    public partial class change_tablename_ApprovalWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApprovalWorkflow");

            migrationBuilder.CreateTable(
                name: "EmpApprovals",
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
                    table.PrimaryKey("PK_EmpApprovals", x => x.id);
                    table.ForeignKey(
                        name: "FK_EmpApprovals_Supervisor_approverIdid",
                        column: x => x.approverIdid,
                        principalTable: "Supervisor",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_EmpApprovals_WorkflowTypes_approvalLevelsid",
                        column: x => x.approvalLevelsid,
                        principalTable: "WorkflowTypes",
                        principalColumn: "id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_EmpApprovals_approvalLevelsid",
                table: "EmpApprovals",
                column: "approvalLevelsid");

            migrationBuilder.CreateIndex(
                name: "IX_EmpApprovals_approverIdid",
                table: "EmpApprovals",
                column: "approverIdid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmpApprovals");

            migrationBuilder.CreateTable(
                name: "ApprovalWorkflow",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    approvalLevelsid = table.Column<int>(type: "int", nullable: true),
                    approverIdid = table.Column<int>(type: "int", nullable: true),
                    createdBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    createdDate = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "(CURDATE())"),
                    createdTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "(CURTIME())"),
                    epf = table.Column<int>(type: "int", nullable: false),
                    lastUpdateBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    lastUpdateDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    lastUpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    level = table.Column<int>(type: "int", nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalWorkflow_approvalLevelsid",
                table: "ApprovalWorkflow",
                column: "approvalLevelsid");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalWorkflow_approverIdid",
                table: "ApprovalWorkflow",
                column: "approverIdid");
        }
    }
}
