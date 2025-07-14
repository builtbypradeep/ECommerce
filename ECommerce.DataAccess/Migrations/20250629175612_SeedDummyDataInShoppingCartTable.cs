using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SeedDummyDataInShoppingCartTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "shoppingCarts",
                columns: new[] { "Id", "ApplicationUserId", "Count", "ProductId" },
                values: new object[] { 1, "32118e51-97e0-4406-8cd7-b1f8a9cdce28", 1, 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "shoppingCarts",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
