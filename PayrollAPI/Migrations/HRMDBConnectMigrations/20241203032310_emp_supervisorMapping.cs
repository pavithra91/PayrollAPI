using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollAPI.Migrations.HRMDBConnectMigrations
{
    /// <inheritdoc />
    public partial class emp_supervisorMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "epf",
                table: "Supervisor");

            migrationBuilder.AddColumn<int>(
                name: "epfid",
                table: "Supervisor",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Supervisor_epfid",
                table: "Supervisor",
                column: "epfid");

            migrationBuilder.AddForeignKey(
                name: "FK_Supervisor_Employee_epfid",
                table: "Supervisor",
                column: "epfid",
                principalTable: "Employee",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Supervisor_Employee_epfid",
                table: "Supervisor");

            migrationBuilder.DropIndex(
                name: "IX_Supervisor_epfid",
                table: "Supervisor");

            migrationBuilder.DropColumn(
                name: "epfid",
                table: "Supervisor");

            migrationBuilder.AddColumn<string>(
                name: "epf",
                table: "Supervisor",
                type: "longtext",
                nullable: false);
        }
    }
}
