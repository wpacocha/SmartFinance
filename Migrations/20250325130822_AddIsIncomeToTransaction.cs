using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartFinance.API.Migrations
{
    /// <inheritdoc />
    public partial class AddIsIncomeToTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsIncome",
                table: "Transactions",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsIncome",
                table: "Transactions");
        }
    }
}
