using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace PayrollAPI.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Calculation",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    companyCode = table.Column<int>(type: "int", nullable: false),
                    sequence = table.Column<int>(type: "int", nullable: false),
                    payCode = table.Column<int>(type: "int", nullable: false),
                    calCode = table.Column<string>(type: "varchar(5)", nullable: true),
                    payCategory = table.Column<string>(type: "varchar(2)", nullable: true),
                    calFormula = table.Column<string>(type: "varchar(500)", nullable: true),
                    calDescription = table.Column<string>(type: "varchar(500)", nullable: true),
                    status = table.Column<bool>(type: "boolean", nullable: false),
                    createdBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    createdDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    lastUpdateBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    lastUpdateDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calculation", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Employee_Data",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    epf = table.Column<int>(type: "int", nullable: false),
                    period = table.Column<int>(type: "int", nullable: false),
                    empName = table.Column<string>(type: "varchar(500)", nullable: true),
                    costCenter = table.Column<string>(type: "varchar(6)", nullable: true),
                    gradeCode = table.Column<int>(type: "int", nullable: false),
                    empGrade = table.Column<string>(type: "varchar(2)", nullable: true),
                    paymentType = table.Column<int>(type: "int", nullable: false),
                    bankCode = table.Column<int>(type: "int", nullable: false),
                    branchCode = table.Column<int>(type: "int", nullable: false),
                    accountNo = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<bool>(type: "boolean", nullable: false),
                    isPaysheetGenerated = table.Column<bool>(type: "boolean", nullable: false),
                    changeBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    changeDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee_Data", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EmpSpecialRate",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    companyCode = table.Column<int>(type: "int", nullable: false),
                    epf = table.Column<int>(type: "int", nullable: false),
                    costcenter = table.Column<string>(type: "varchar(50)", nullable: true),
                    payCode = table.Column<int>(type: "int", nullable: false),
                    calCode = table.Column<string>(type: "varchar(5)", nullable: true),
                    rate = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    status = table.Column<bool>(type: "boolean", nullable: false),
                    createdBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    createdDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    lastUpdateBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    lastUpdateDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpSpecialRate", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EPF_ETF",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    period = table.Column<int>(type: "int", nullable: false),
                    epf = table.Column<int>(type: "int", nullable: false),
                    empName = table.Column<string>(type: "varchar(200)", nullable: true),
                    grade = table.Column<string>(type: "varchar(2)", nullable: false),
                    emp_contribution = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    comp_contribution = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    etf = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    createdBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    createdDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EPF_ETF", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PayCode",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    companyCode = table.Column<int>(type: "int", nullable: false),
                    payCode = table.Column<int>(type: "int", nullable: false),
                    calCode = table.Column<string>(type: "varchar(5)", nullable: true),
                    payCategory = table.Column<string>(type: "varchar(2)", nullable: true),
                    description = table.Column<string>(type: "varchar(100)", nullable: true),
                    isTaxableGross = table.Column<bool>(type: "boolean", nullable: false),
                    rate = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    createdBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    createdDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    lastUpdateBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    lastUpdateDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayCode", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Payroll_Data",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    period = table.Column<int>(type: "int", nullable: false),
                    epf = table.Column<int>(type: "int", nullable: false),
                    othours = table.Column<float>(type: "float", nullable: false),
                    payCategory = table.Column<string>(type: "varchar(2)", nullable: true),
                    payCode = table.Column<int>(type: "int", nullable: false),
                    calCode = table.Column<string>(type: "varchar(5)", nullable: true),
                    paytype = table.Column<ulong>(type: "bit", nullable: false),
                    costcenter = table.Column<string>(type: "varchar(6)", nullable: true),
                    payCodeType = table.Column<string>(type: "varchar(2)", nullable: true),
                    amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    balanceamount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    displayOnPaySheet = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payroll_Data", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Payrun",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    period = table.Column<int>(type: "int", nullable: false),
                    noOfEmployees = table.Column<int>(type: "int", nullable: false),
                    noOfRecords = table.Column<int>(type: "int", nullable: false),
                    approvedBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    approvedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    payrunStatus = table.Column<string>(type: "varchar(10)", nullable: true),
                    payrunBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    payrunDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payrun", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Special_Tax_Emp",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    companyCode = table.Column<int>(type: "int", nullable: false),
                    epf = table.Column<int>(type: "int", nullable: false),
                    costcenter = table.Column<string>(type: "varchar(50)", nullable: true),
                    calFormula = table.Column<string>(type: "varchar(500)", nullable: true),
                    status = table.Column<bool>(type: "boolean", nullable: false),
                    createdBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    createdDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    lastUpdateBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    lastUpdateDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Special_Tax_Emp", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Tax_Calculation",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    range = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    calFormula = table.Column<string>(type: "varchar(500)", nullable: true),
                    description = table.Column<string>(type: "varchar(500)", nullable: true),
                    status = table.Column<bool>(type: "boolean", nullable: false),
                    createdBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    createdDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    lastUpdateBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    lastUpdateDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tax_Calculation", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Temp_Employee",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    epf = table.Column<int>(type: "int", nullable: false),
                    period = table.Column<int>(type: "int", nullable: false),
                    empName = table.Column<string>(type: "varchar(500)", nullable: true),
                    costCenter = table.Column<string>(type: "varchar(6)", nullable: true),
                    gradeCode = table.Column<int>(type: "int", nullable: false),
                    empGrade = table.Column<string>(type: "varchar(2)", nullable: true),
                    paymentType = table.Column<int>(type: "int", nullable: false),
                    bankCode = table.Column<int>(type: "int", nullable: false),
                    branchCode = table.Column<int>(type: "int", nullable: false),
                    accountNo = table.Column<int>(type: "int", nullable: false),
                    createdBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    createdDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Temp_Employee", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Temp_Payroll",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    period = table.Column<int>(type: "int", nullable: false),
                    epf = table.Column<int>(type: "int", nullable: false),
                    othours = table.Column<float>(type: "float", nullable: false),
                    payCategory = table.Column<string>(type: "varchar(2)", nullable: true),
                    payCode = table.Column<int>(type: "int", nullable: false),
                    calCode = table.Column<string>(type: "varchar(5)", nullable: true),
                    paytype = table.Column<ulong>(type: "bit", nullable: false),
                    costcenter = table.Column<string>(type: "varchar(6)", nullable: true),
                    payCodeType = table.Column<string>(type: "varchar(2)", nullable: true),
                    amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    balanceamount = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Temp_Payroll", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TotPayCode",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    companyCode = table.Column<int>(type: "int", nullable: false),
                    payCode = table.Column<int>(type: "int", nullable: false),
                    calCode = table.Column<string>(type: "varchar(5)", nullable: true),
                    period = table.Column<int>(type: "int", nullable: false),
                    total = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    createdBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    createdDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TotPayCode", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Calculation");

            migrationBuilder.DropTable(
                name: "Employee_Data");

            migrationBuilder.DropTable(
                name: "EmpSpecialRate");

            migrationBuilder.DropTable(
                name: "EPF_ETF");

            migrationBuilder.DropTable(
                name: "PayCode");

            migrationBuilder.DropTable(
                name: "Payroll_Data");

            migrationBuilder.DropTable(
                name: "Payrun");

            migrationBuilder.DropTable(
                name: "Special_Tax_Emp");

            migrationBuilder.DropTable(
                name: "Tax_Calculation");

            migrationBuilder.DropTable(
                name: "Temp_Employee");

            migrationBuilder.DropTable(
                name: "Temp_Payroll");

            migrationBuilder.DropTable(
                name: "TotPayCode");
        }
    }
}
