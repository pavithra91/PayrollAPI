using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollAPI.Migrations
{
    /// <inheritdoc />
    public partial class _020120231 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "tokenID",
                table: "LoginInfo",
                type: "varchar(300)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "refreshToken",
                table: "LoginInfo",
                type: "varchar(300)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "tokenID",
                table: "LoginInfo",
                type: "varchar(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(300)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "refreshToken",
                table: "LoginInfo",
                type: "varchar(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(300)",
                oldNullable: true);
        }
    }
}
