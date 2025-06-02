using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseManagement.Migrations
{
    /// <inheritdoc />
    public partial class fixDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseDetails_Products_ProId",
                table: "WarehouseDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseDetails_Warehouses_WarehouseId",
                table: "WarehouseDetails");

            migrationBuilder.DropIndex(
                name: "IX_WarehouseDetails_WarehouseId",
                table: "WarehouseDetails");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "WarehouseDetails");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseDetails_WareId",
                table: "WarehouseDetails",
                column: "WareId");

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseDetails_Products_ProId",
                table: "WarehouseDetails",
                column: "ProId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseDetails_Warehouses_WareId",
                table: "WarehouseDetails",
                column: "WareId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseDetails_Products_ProId",
                table: "WarehouseDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseDetails_Warehouses_WareId",
                table: "WarehouseDetails");

            migrationBuilder.DropIndex(
                name: "IX_WarehouseDetails_WareId",
                table: "WarehouseDetails");

            migrationBuilder.AddColumn<int>(
                name: "WarehouseId",
                table: "WarehouseDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseDetails_WarehouseId",
                table: "WarehouseDetails",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseDetails_Products_ProId",
                table: "WarehouseDetails",
                column: "ProId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseDetails_Warehouses_WarehouseId",
                table: "WarehouseDetails",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id");
        }
    }
}
