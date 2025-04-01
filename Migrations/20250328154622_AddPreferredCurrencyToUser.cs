using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartFinance.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPreferredCurrencyToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PrefferedCurrency",
                table: "Users",
                newName: "PreferredCurrency");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PreferredCurrency",
                table: "Users",
                newName: "PrefferedCurrency");
        }
    }
}
