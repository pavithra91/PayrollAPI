using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace PayrollAPI.Migrations.HRMDBConnectMigrations
{
    /// <inheritdoc />
    public partial class add_leave_tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LeaveType",
                columns: table => new
                {
                    leaveTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    leaveTypeName = table.Column<string>(type: "varchar(50)", nullable: false),
                    description = table.Column<string>(type: "varchar(250)", nullable: false),
                    maxDays = table.Column<decimal>(type: "decimal(2,1)", nullable: false),
                    carryForwardAllowed = table.Column<bool>(type: "boolean", nullable: false),
                    createdBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    createdDate = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "(CURDATE())"),
                    createdTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "(CURTIME())"),
                    lastUpdateBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    lastUpdateDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    lastUpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveType", x => x.leaveTypeId);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LeaveBalance",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    epf = table.Column<int>(type: "int", nullable: false),
                    year = table.Column<int>(type: "int", nullable: false),
                    leaveTypeId = table.Column<int>(type: "int", nullable: false),
                    allocatedLeaves = table.Column<decimal>(type: "decimal(2,1)", nullable: false),
                    usedLeaves = table.Column<decimal>(type: "decimal(2,1)", nullable: false),
                    remainingLeaves = table.Column<decimal>(type: "decimal(2,1)", nullable: false),
                    carryForwardLeaves = table.Column<decimal>(type: "decimal(2,1)", nullable: false),
                    createdBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    createdDate = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "(CURDATE())"),
                    createdTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "(CURTIME())"),
                    lastUpdateBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    lastUpdateDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    lastUpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveBalance", x => x.id);
                    table.ForeignKey(
                        name: "FK_LeaveBalance_LeaveType_leaveTypeId",
                        column: x => x.leaveTypeId,
                        principalTable: "LeaveType",
                        principalColumn: "leaveTypeId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LeaveRequest",
                columns: table => new
                {
                    leaveRequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    epf = table.Column<int>(type: "int", nullable: false),
                    leaveTypeId = table.Column<int>(type: "int", nullable: false),
                    startDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    endDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    lieuLeaveDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    reason = table.Column<string>(type: "varchar(250)", nullable: true),
                    isHalfDay = table.Column<bool>(type: "boolean", nullable: false),
                    status = table.Column<string>(type: "varchar(10)", nullable: false),
                    actionedBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    actionedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    actionedTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    createdDate = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "(CURDATE())"),
                    createdTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "(CURTIME())"),
                    lastUpdateBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    lastUpdateDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    lastUpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveRequest", x => x.leaveRequestId);
                    table.ForeignKey(
                        name: "FK_LeaveRequest_LeaveType_leaveTypeId",
                        column: x => x.leaveTypeId,
                        principalTable: "LeaveType",
                        principalColumn: "leaveTypeId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveBalance_leaveTypeId",
                table: "LeaveBalance",
                column: "leaveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequest_leaveTypeId",
                table: "LeaveRequest",
                column: "leaveTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LeaveBalance");

            migrationBuilder.DropTable(
                name: "LeaveRequest");

            migrationBuilder.DropTable(
                name: "LeaveType");
        }
    }
}
