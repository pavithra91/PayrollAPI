using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace PayrollAPI.Migrations
{
    /// <inheritdoc />
    public partial class _20230112_CPP29 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "company",
                table: "Temp_Payroll",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "plant",
                table: "Temp_Payroll",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "company",
                table: "Temp_Employee",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "plant",
                table: "Temp_Employee",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "company",
                table: "Payroll_Data",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "plant",
                table: "Payroll_Data",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "accountNo",
                table: "Employee_Data",
                type: "varchar(15)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "company",
                table: "Employee_Data",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "plant",
                table: "Employee_Data",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Unrecovered",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    companyCode = table.Column<int>(type: "int", nullable: false),
                    period = table.Column<int>(type: "int", nullable: false),
                    epf = table.Column<string>(type: "longtext", nullable: false),
                    payCategory = table.Column<string>(type: "varchar(2)", nullable: true),
                    payCode = table.Column<int>(type: "int", nullable: false),
                    calCode = table.Column<string>(type: "varchar(5)", nullable: true),
                    costcenter = table.Column<string>(type: "varchar(6)", nullable: true),
                    amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    createdDate = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "(CURDATE())"),
                    createdTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "(CURTIME())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Unrecovered", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Unrecovered");

            migrationBuilder.DropColumn(
                name: "company",
                table: "Temp_Payroll");

            migrationBuilder.DropColumn(
                name: "plant",
                table: "Temp_Payroll");

            migrationBuilder.DropColumn(
                name: "company",
                table: "Temp_Employee");

            migrationBuilder.DropColumn(
                name: "plant",
                table: "Temp_Employee");

            migrationBuilder.DropColumn(
                name: "company",
                table: "Payroll_Data");

            migrationBuilder.DropColumn(
                name: "plant",
                table: "Payroll_Data");

            migrationBuilder.DropColumn(
                name: "company",
                table: "Employee_Data");

            migrationBuilder.DropColumn(
                name: "plant",
                table: "Employee_Data");

            migrationBuilder.AlterColumn<int>(
                name: "accountNo",
                table: "Employee_Data",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "varchar(15)",
                oldNullable: true);
        }
    }
}
