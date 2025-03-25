using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartFinance.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPrefferedCurrencyToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PrefferedCurrency",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrefferedCurrency",
                table: "Users");
        }
    }
}
