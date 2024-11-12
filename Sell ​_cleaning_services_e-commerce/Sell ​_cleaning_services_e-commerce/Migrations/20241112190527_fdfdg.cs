using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sell__cleaning_services_e_commerce.Migrations
{
    /// <inheritdoc />
    public partial class fdfdg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                table: "EmailLogs");

            migrationBuilder.DropColumn(
                name: "IsSuccess",
                table: "EmailLogs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                table: "EmailLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSuccess",
                table: "EmailLogs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
