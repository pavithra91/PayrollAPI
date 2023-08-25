using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace PayrollAPI.Migrations
{
    /// <inheritdoc />
    public partial class _20230825 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    userID = table.Column<string>(type: "varchar(10)", nullable: true),
                    epf = table.Column<int>(type: "int", nullable: false),
                    empName = table.Column<string>(type: "varchar(60)", nullable: true),
                    costCenter = table.Column<string>(type: "varchar(6)", nullable: true),
                    role = table.Column<string>(type: "varchar(6)", nullable: true),
                    pwdSalt = table.Column<string>(type: "varchar(50)", nullable: true),
                    pwdHash = table.Column<string>(type: "varchar(200)", nullable: true),
                    status = table.Column<bool>(type: "boolean", nullable: false),
                    createdBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    createdDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    lastUpdateBy = table.Column<string>(type: "varchar(10)", nullable: true),
                    lastUpdateDate = table.Column<DateTime>(type: "datetime", nullable: false)
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
                name: "User");
        }
    }
}
