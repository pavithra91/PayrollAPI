using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace PayrollAPI.Migrations.HRMDBConnectMigrations
{
    /// <inheritdoc />
    public partial class add_rateTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "reservationCategoryid",
                table: "Reservation",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ReservationCategory",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    categoryName = table.Column<string>(type: "varchar(10)", nullable: true),
                    createdBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    createdDate = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "(CURDATE())"),
                    createdTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "(CURTIME())"),
                    lastUpdateBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    lastUpdateDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    lastUpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationCategory", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "BungalowRates",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    bungalowid = table.Column<int>(type: "int", nullable: false),
                    categoryid = table.Column<int>(type: "int", nullable: false),
                    perDayCost = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    createdBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    createdDate = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "(CURDATE())"),
                    createdTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "(CURTIME())"),
                    lastUpdateBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    lastUpdateDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    lastUpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BungalowRates", x => x.id);
                    table.ForeignKey(
                        name: "FK_BungalowRates_Bungalow_bungalowid",
                        column: x => x.bungalowid,
                        principalTable: "Bungalow",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BungalowRates_ReservationCategory_categoryid",
                        column: x => x.categoryid,
                        principalTable: "ReservationCategory",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_reservationCategoryid",
                table: "Reservation",
                column: "reservationCategoryid");

            migrationBuilder.CreateIndex(
                name: "IX_BungalowRates_bungalowid",
                table: "BungalowRates",
                column: "bungalowid");

            migrationBuilder.CreateIndex(
                name: "IX_BungalowRates_categoryid",
                table: "BungalowRates",
                column: "categoryid");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_ReservationCategory_reservationCategoryid",
                table: "Reservation",
                column: "reservationCategoryid",
                principalTable: "ReservationCategory",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_ReservationCategory_reservationCategoryid",
                table: "Reservation");

            migrationBuilder.DropTable(
                name: "BungalowRates");

            migrationBuilder.DropTable(
                name: "ReservationCategory");

            migrationBuilder.DropIndex(
                name: "IX_Reservation_reservationCategoryid",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "reservationCategoryid",
                table: "Reservation");
        }
    }
}
