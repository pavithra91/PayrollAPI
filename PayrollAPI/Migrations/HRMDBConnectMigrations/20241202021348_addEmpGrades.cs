using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace PayrollAPI.Migrations.HRMDBConnectMigrations
{
    /// <inheritdoc />
    public partial class addEmpGrades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "grade",
                table: "Employee");

            migrationBuilder.AlterColumn<DateTime>(
                name: "lastUpdateTime",
                table: "Employee",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "lastUpdateDate",
                table: "Employee",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AddColumn<int>(
                name: "EmployeeGradeid",
                table: "Employee",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "empGradeid",
                table: "Employee",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EmployeeGrade",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    gradeCode = table.Column<string>(type: "varchar(5)", nullable: true),
                    description = table.Column<string>(type: "varchar(20)", nullable: true),
                    createdBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    createdDate = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "(CURDATE())"),
                    createdTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "(CURTIME())"),
                    lastUpdateBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    lastUpdateDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    lastUpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeGrade", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_empGradeid",
                table: "Employee",
                column: "empGradeid");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_EmployeeGradeid",
                table: "Employee",
                column: "EmployeeGradeid");

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_EmployeeGrade_EmployeeGradeid",
                table: "Employee",
                column: "EmployeeGradeid",
                principalTable: "EmployeeGrade",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_Employee_empGradeid",
                table: "Employee",
                column: "empGradeid",
                principalTable: "Employee",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employee_EmployeeGrade_EmployeeGradeid",
                table: "Employee");

            migrationBuilder.DropForeignKey(
                name: "FK_Employee_Employee_empGradeid",
                table: "Employee");

            migrationBuilder.DropTable(
                name: "EmployeeGrade");

            migrationBuilder.DropIndex(
                name: "IX_Employee_empGradeid",
                table: "Employee");

            migrationBuilder.DropIndex(
                name: "IX_Employee_EmployeeGradeid",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "EmployeeGradeid",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "empGradeid",
                table: "Employee");

            migrationBuilder.AlterColumn<DateTime>(
                name: "lastUpdateTime",
                table: "Employee",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "lastUpdateDate",
                table: "Employee",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "grade",
                table: "Employee",
                type: "varchar(20)",
                nullable: true);
        }
    }
}
