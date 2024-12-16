using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollAPI.Migrations.HRMDBConnectMigrations
{
    /// <inheritdoc />
    public partial class add_tempApproval : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "tempApproval",
                table: "LeaveApproval",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "tempApproval",
                table: "LeaveApproval");
        }
    }
}
