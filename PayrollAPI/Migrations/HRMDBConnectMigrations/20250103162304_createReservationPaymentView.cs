using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollAPI.Migrations.HRMDBConnectMigrations
{
    /// <inheritdoc />
    public partial class createReservationPaymentView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GetReservationPaymentDetails",
                columns: table => new
                {
                    reservationId = table.Column<int>(type: "int", nullable: false),
                    epf = table.Column<string>(type: "longtext", nullable: false),
                    checkInDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    checkOutDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    categoryName = table.Column<string>(type: "longtext", nullable: false),
                    status = table.Column<string>(type: "longtext", nullable: false),
                    amount = table.Column<float>(type: "float", nullable: false),
                    chargeType = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                })
                .Annotation("MySQL:Charset", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GetReservationPaymentDetails");
        }
    }
}
