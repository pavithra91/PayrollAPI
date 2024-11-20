using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollAPI.Migrations.HRMDBConnectMigrations
{
    /// <inheritdoc />
    public partial class correct_decimal_values : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "maxDays",
                table: "LeaveType",
                type: "decimal(4,1)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(2,1)");

            migrationBuilder.AlterColumn<decimal>(
                name: "usedLeaves",
                table: "LeaveBalance",
                type: "decimal(4,1)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(2,1)");

            migrationBuilder.AlterColumn<decimal>(
                name: "remainingLeaves",
                table: "LeaveBalance",
                type: "decimal(4,1)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(2,1)");

            migrationBuilder.AlterColumn<decimal>(
                name: "carryForwardLeaves",
                table: "LeaveBalance",
                type: "decimal(4,1)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(2,1)");

            migrationBuilder.AlterColumn<decimal>(
                name: "allocatedLeaves",
                table: "LeaveBalance",
                type: "decimal(4,1)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(2,1)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "maxDays",
                table: "LeaveType",
                type: "decimal(2,1)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(4,1)");

            migrationBuilder.AlterColumn<decimal>(
                name: "usedLeaves",
                table: "LeaveBalance",
                type: "decimal(2,1)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(4,1)");

            migrationBuilder.AlterColumn<decimal>(
                name: "remainingLeaves",
                table: "LeaveBalance",
                type: "decimal(2,1)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(4,1)");

            migrationBuilder.AlterColumn<decimal>(
                name: "carryForwardLeaves",
                table: "LeaveBalance",
                type: "decimal(2,1)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(4,1)");

            migrationBuilder.AlterColumn<decimal>(
                name: "allocatedLeaves",
                table: "LeaveBalance",
                type: "decimal(2,1)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(4,1)");
        }
    }
}
