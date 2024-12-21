using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace PayrollAPI.Migrations.HRMDBConnectMigrations
{
    /// <inheritdoc />
    public partial class bungalow_bookingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bungalow",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    companyCode = table.Column<int>(type: "int", nullable: false),
                    bungalowName = table.Column<string>(type: "varchar(100)", nullable: true),
                    noOfRooms = table.Column<int>(type: "int", nullable: false),
                    perDayCost = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    maxBookingPeriod = table.Column<int>(type: "int", nullable: false),
                    isCloded = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    reopenDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    createdBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    createdDate = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "(CURDATE())"),
                    createdTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "(CURTIME())"),
                    lastUpdateBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    lastUpdateDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    lastUpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bungalow", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Reservation",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    companyCode = table.Column<int>(type: "int", nullable: false),
                    employeeid = table.Column<int>(type: "int", nullable: false),
                    bungalowid = table.Column<int>(type: "int", nullable: false),
                    bookingType = table.Column<int>(type: "int", nullable: false),
                    checkInDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    checkOutDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    noOfAdults = table.Column<int>(type: "int", nullable: false),
                    noOfChildren = table.Column<int>(type: "int", nullable: false),
                    totalPax = table.Column<int>(type: "int", nullable: false),
                    contactNumber_1 = table.Column<string>(type: "varchar(15)", nullable: false),
                    contactNumber_2 = table.Column<string>(type: "varchar(15)", nullable: true),
                    bookingStatus = table.Column<int>(type: "int", nullable: false),
                    createdBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    createdDate = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "(CURDATE())"),
                    createdTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "(CURTIME())"),
                    lastUpdateBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    lastUpdateDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    lastUpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservation", x => x.id);
                    table.ForeignKey(
                        name: "FK_Reservation_Bungalow_bungalowid",
                        column: x => x.bungalowid,
                        principalTable: "Bungalow",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservation_Employee_employeeid",
                        column: x => x.employeeid,
                        principalTable: "Employee",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RaffleDraw",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    reservationID = table.Column<int>(type: "int", nullable: false),
                    rank = table.Column<int>(type: "int", nullable: false),
                    createdDate = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "(CURDATE())"),
                    createdTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "(CURTIME())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaffleDraw", x => x.id);
                    table.ForeignKey(
                        name: "FK_RaffleDraw_Reservation_reservationID",
                        column: x => x.reservationID,
                        principalTable: "Reservation",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_RaffleDraw_reservationID",
                table: "RaffleDraw",
                column: "reservationID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_bungalowid",
                table: "Reservation",
                column: "bungalowid");

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_employeeid",
                table: "Reservation",
                column: "employeeid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RaffleDraw");

            migrationBuilder.DropTable(
                name: "Reservation");

            migrationBuilder.DropTable(
                name: "Bungalow");
        }
    }
}
