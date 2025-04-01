using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartFinance.API.Migrations
{
    /// <inheritdoc />
    public partial class InitWithMonthUserEtc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Months_MonthId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_MonthId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "MonthId",
                table: "Transactions");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Months",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Months_UserId",
                table: "Months",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Months_Users_UserId",
                table: "Months",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Months_Users_UserId",
                table: "Months");

            migrationBuilder.DropIndex(
                name: "IX_Months_UserId",
                table: "Months");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Months");

            migrationBuilder.AddColumn<int>(
                name: "MonthId",
                table: "Transactions",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_MonthId",
                table: "Transactions",
                column: "MonthId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Months_MonthId",
                table: "Transactions",
                column: "MonthId",
                principalTable: "Months",
                principalColumn: "Id");
        }
    }
}
