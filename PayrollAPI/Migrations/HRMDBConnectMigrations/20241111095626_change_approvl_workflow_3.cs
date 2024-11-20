using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollAPI.Migrations.HRMDBConnectMigrations
{
    /// <inheritdoc />
    public partial class change_approvl_workflow_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmpApprovals_EmpApprovals_EmpApprovalsid",
                table: "EmpApprovals");

            migrationBuilder.DropIndex(
                name: "IX_EmpApprovals_EmpApprovalsid",
                table: "EmpApprovals");

            migrationBuilder.DropColumn(
                name: "EmpApprovalsid",
                table: "EmpApprovals");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmpApprovalsid",
                table: "EmpApprovals",
                type: "int",
                nullable: true);

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
        }
    }
}
