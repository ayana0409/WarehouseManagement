using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseManagement.Migrations
{
    /// <inheritdoc />
    public partial class changeFieldStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StockQuantity",
                table: "ImportDetails");

            migrationBuilder.AddColumn<double>(
                name: "UnallocatedStock",
                table: "Products",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnallocatedStock",
                table: "Products");

            migrationBuilder.AddColumn<double>(
                name: "StockQuantity",
                table: "ImportDetails",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
