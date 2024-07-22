using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace PayrollAPI.Migrations
{
    /// <inheritdoc />
    public partial class _18122023Update_location : Migration
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
                    createdDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(CURDATE())"),
                    createdTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(CURTIME())"),
                    lastUpdateBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    lastUpdateDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    lastUpdateTime = table.Column<DateTime>(type: "datetime", nullable: false)
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
                    companyCode = table.Column<int>(type: "int", nullable: false),
                    location = table.Column<int>(type: "int", nullable: false),
                    epf = table.Column<string>(type: "varchar(6)", nullable: false),
                    period = table.Column<int>(type: "int", nullable: false),
                    empName = table.Column<string>(type: "varchar(500)", nullable: true),
                    costCenter = table.Column<string>(type: "varchar(6)", nullable: true),
                    gradeCode = table.Column<int>(type: "int", nullable: false),
                    empGrade = table.Column<string>(type: "varchar(5)", nullable: true),
                    paymentType = table.Column<int>(type: "int", nullable: false),
                    bankCode = table.Column<int>(type: "int", nullable: false),
                    branchCode = table.Column<int>(type: "int", nullable: false),
                    accountNo = table.Column<string>(type: "varchar(15)", nullable: true),
                    status = table.Column<bool>(type: "boolean", nullable: true, defaultValueSql: "1"),
                    isPaysheetGenerated = table.Column<bool>(type: "boolean", nullable: true, defaultValueSql: "1"),
                    changeBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    changeDate = table.Column<DateTime>(type: "datetime", nullable: true)
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
                    epf = table.Column<string>(type: "varchar(6)", nullable: false),
                    costCenter = table.Column<string>(type: "varchar(50)", nullable: true),
                    location = table.Column<int>(type: "int", nullable: true),
                    payCode = table.Column<int>(type: "int", nullable: false),
                    calCode = table.Column<string>(type: "varchar(5)", nullable: true),
                    rate = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    status = table.Column<bool>(type: "boolean", nullable: false),
                    createdBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    createdDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(CURDATE())"),
                    createdTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(CURTIME())"),
                    lastUpdateBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    lastUpdateDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    lastUpdateTime = table.Column<DateTime>(type: "datetime", nullable: false)
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
                    epf = table.Column<string>(type: "varchar(6)", nullable: false),
                    empName = table.Column<string>(type: "varchar(200)", nullable: true),
                    grade = table.Column<string>(type: "varchar(2)", nullable: false),
                    emp_contribution = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    comp_contribution = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    etf = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    createdBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    createdDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(CURDATE())"),
                    createdTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(CURTIME())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EPF_ETF", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LoginInfo",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    userID = table.Column<string>(type: "varchar(10)", nullable: true),
                    tokenID = table.Column<string>(type: "varchar(100)", nullable: true),
                    refreshToken = table.Column<string>(type: "varchar(100)", nullable: true),
                    loginDateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    loginTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    isActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginInfo", x => x.id);
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
                    createdDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(CURDATE())"),
                    createdTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(CURTIME())"),
                    lastUpdateBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    lastUpdateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    lastUpdateTime = table.Column<DateTime>(type: "datetime", nullable: true)
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
                    companyCode = table.Column<int>(type: "int", nullable: false),
                    location = table.Column<int>(type: "int", nullable: false),
                    period = table.Column<int>(type: "int", nullable: false),
                    epf = table.Column<string>(type: "varchar(6)", nullable: false),
                    othours = table.Column<float>(type: "float", nullable: false),
                    payCategory = table.Column<string>(type: "varchar(2)", nullable: true),
                    payCode = table.Column<int>(type: "int", nullable: false),
                    calCode = table.Column<string>(type: "varchar(5)", nullable: true),
                    paytype = table.Column<string>(type: "varchar(1)", nullable: true),
                    costCenter = table.Column<string>(type: "varchar(6)", nullable: true),
                    payCodeType = table.Column<string>(type: "varchar(2)", nullable: true),
                    amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    balanceAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    epfConRate = table.Column<float>(type: "float", nullable: false),
                    epfContribution = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    taxConRate = table.Column<float>(type: "float", nullable: false),
                    taxContribution = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    displayOnPaySheet = table.Column<bool>(type: "boolean", nullable: true, defaultValueSql: "1")
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
                    companyCode = table.Column<int>(type: "int", nullable: false),
                    period = table.Column<int>(type: "int", nullable: false),
                    noOfEmployees = table.Column<int>(type: "int", nullable: false),
                    noOfRecords = table.Column<int>(type: "int", nullable: false),
                    approvedBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    approvedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    approvedTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    payrunStatus = table.Column<string>(type: "varchar(10)", nullable: true),
                    payrunBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    payrunDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    payrunTime = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payrun", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SAPTotPayCode",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    companyCode = table.Column<int>(type: "int", nullable: false),
                    payCode = table.Column<int>(type: "int", nullable: false),
                    payType = table.Column<string>(type: "varchar(1)", nullable: true),
                    calCode = table.Column<string>(type: "varchar(5)", nullable: true),
                    period = table.Column<int>(type: "int", nullable: false),
                    totalAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    totCount = table.Column<int>(type: "int", nullable: false),
                    createdBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    createdDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(CURDATE())"),
                    createdTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(CURTIME())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SAPTotPayCode", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Special_Tax_Emp",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    companyCode = table.Column<int>(type: "int", nullable: false),
                    epf = table.Column<string>(type: "varchar(6)", nullable: false),
                    costCenter = table.Column<string>(type: "varchar(50)", nullable: true),
                    location = table.Column<int>(type: "int", nullable: true),
                    calFormula = table.Column<string>(type: "varchar(500)", nullable: true),
                    status = table.Column<bool>(type: "boolean", nullable: false),
                    createdBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    createdDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(CURDATE())"),
                    createdTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(CURTIME())"),
                    lastUpdateBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    lastUpdateDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    lastUpdateTime = table.Column<DateTime>(type: "datetime", nullable: false)
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
                    createdDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(CURDATE())"),
                    createdTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(CURTIME())"),
                    lastUpdateBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    lastUpdateDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    lastUpdateTime = table.Column<DateTime>(type: "datetime", nullable: false)
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
                    companyCode = table.Column<int>(type: "int", nullable: false),
                    location = table.Column<int>(type: "int", nullable: false),
                    epf = table.Column<string>(type: "varchar(6)", nullable: false),
                    period = table.Column<int>(type: "int", nullable: false),
                    empName = table.Column<string>(type: "varchar(100)", nullable: true),
                    costCenter = table.Column<string>(type: "varchar(6)", nullable: true),
                    gradeCode = table.Column<int>(type: "int", nullable: false),
                    empGrade = table.Column<string>(type: "varchar(5)", nullable: true),
                    paymentType = table.Column<int>(type: "int", nullable: false),
                    bankCode = table.Column<int>(type: "int", nullable: false),
                    branchCode = table.Column<int>(type: "int", nullable: false),
                    accountNo = table.Column<string>(type: "varchar(15)", nullable: true),
                    createdBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    createdDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(CURDATE())"),
                    createdTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(CURTIME())")
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
                    companyCode = table.Column<int>(type: "int", nullable: false),
                    location = table.Column<int>(type: "int", nullable: false),
                    period = table.Column<int>(type: "int", nullable: false),
                    epf = table.Column<string>(type: "varchar(6)", nullable: false),
                    othours = table.Column<float>(type: "float", nullable: false),
                    payCategory = table.Column<string>(type: "varchar(2)", nullable: true),
                    payCode = table.Column<int>(type: "int", nullable: false),
                    calCode = table.Column<string>(type: "varchar(5)", nullable: true),
                    paytype = table.Column<string>(type: "varchar(1)", nullable: true),
                    costCenter = table.Column<string>(type: "varchar(6)", nullable: true),
                    payCodeType = table.Column<string>(type: "varchar(2)", nullable: true),
                    amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    balanceamount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    epfConRate = table.Column<float>(type: "float", nullable: false),
                    taxConRate = table.Column<float>(type: "float", nullable: false),
                    createdDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(CURDATE())"),
                    createdTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(CURTIME())")
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
                    createdDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(CURDATE())"),
                    createdTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(CURTIME())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TotPayCode", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Unrecovered",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    companyCode = table.Column<int>(type: "int", nullable: false),
                    location = table.Column<int>(type: "int", nullable: false),
                    period = table.Column<int>(type: "int", nullable: false),
                    epf = table.Column<string>(type: "longtext", nullable: false),
                    payCategory = table.Column<string>(type: "varchar(2)", nullable: true),
                    payCode = table.Column<int>(type: "int", nullable: false),
                    calCode = table.Column<string>(type: "varchar(5)", nullable: true),
                    costCenter = table.Column<string>(type: "varchar(6)", nullable: true),
                    amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    createdDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(CURDATE())"),
                    createdTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(CURTIME())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Unrecovered", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    userID = table.Column<string>(type: "varchar(10)", nullable: true),
                    epf = table.Column<string>(type: "varchar(6)", nullable: false),
                    empName = table.Column<string>(type: "varchar(60)", nullable: true),
                    costCenter = table.Column<string>(type: "varchar(6)", nullable: true),
                    role = table.Column<string>(type: "varchar(6)", nullable: true),
                    pwdSalt = table.Column<string>(type: "varchar(50)", nullable: true),
                    pwdHash = table.Column<string>(type: "varchar(200)", nullable: true),
                    status = table.Column<bool>(type: "boolean", nullable: false),
                    createdBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    createdDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(CURDATE())"),
                    createdTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(CURTIME())"),
                    lastUpdateBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    lastUpdateDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    lastUpdateTime = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.id);
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
                name: "LoginInfo");

            migrationBuilder.DropTable(
                name: "PayCode");

            migrationBuilder.DropTable(
                name: "Payroll_Data");

            migrationBuilder.DropTable(
                name: "Payrun");

            migrationBuilder.DropTable(
                name: "SAPTotPayCode");

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

            migrationBuilder.DropTable(
                name: "Unrecovered");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
