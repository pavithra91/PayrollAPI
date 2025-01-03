using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollAPI.Migrations.HRMDBConnectMigrations
{
    /// <inheritdoc />
    public partial class createReservationPaymentView_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GetReservationPaymentDetails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GetReservationPaymentDetails",
                columns: table => new
                {
                    amount = table.Column<float>(type: "float", nullable: false),
                    categoryName = table.Column<string>(type: "longtext", nullable: false),
                    chargeType = table.Column<string>(type: "longtext", nullable: false),
                    checkInDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    checkOutDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    epf = table.Column<string>(type: "longtext", nullable: false),
                    reservationId = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                })
                .Annotation("MySQL:Charset", "utf8mb4");
        }
    }
}
