using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace PayrollAPI.Migrations.HRMDBConnectMigrations
{
    /// <inheritdoc />
    public partial class addNotifcations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    epf = table.Column<int>(type: "int", nullable: false),
                    description = table.Column<string>(type: "longtext", nullable: true),
                    markAsRead = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    type = table.Column<int>(type: "int", nullable: false),
                    target = table.Column<string>(type: "longtext", nullable: true),
                    status = table.Column<string>(type: "longtext", nullable: true),
                    createdDate = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "(CURDATE())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notification");
        }
    }
}
