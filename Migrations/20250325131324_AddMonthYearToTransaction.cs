using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartFinance.API.Migrations
{
    /// <inheritdoc />
    public partial class AddMonthYearToTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Month",
                table: "Transactions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Transactions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Month",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Transactions");
        }
    }
}
