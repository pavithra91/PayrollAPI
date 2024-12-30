using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace PayrollAPI.Migrations.HRMDBConnectMigrations
{
    /// <inheritdoc />
    public partial class addCancellationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "lastUpdateBy",
                table: "RaffleDraw",
                type: "varchar(10)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "lastUpdateDate",
                table: "RaffleDraw",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "lastUpdateTime",
                table: "RaffleDraw",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "CancellationCharges",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    reservationID = table.Column<int>(type: "int", nullable: false),
                    cancellation_Policy = table.Column<int>(type: "int", nullable: false),
                    cancellationFees = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    createdBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    createdDate = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "(CURDATE())"),
                    createdTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "(CURTIME())"),
                    lastUpdateBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    lastUpdateDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    lastUpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CancellationCharges", x => x.id);
                    table.ForeignKey(
                        name: "FK_CancellationCharges_Reservation_reservationID",
                        column: x => x.reservationID,
                        principalTable: "Reservation",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CancellationCharges_reservationID",
                table: "CancellationCharges",
                column: "reservationID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CancellationCharges");

            migrationBuilder.DropColumn(
                name: "lastUpdateBy",
                table: "RaffleDraw");

            migrationBuilder.DropColumn(
                name: "lastUpdateDate",
                table: "RaffleDraw");

            migrationBuilder.DropColumn(
                name: "lastUpdateTime",
                table: "RaffleDraw");
        }
    }
}
