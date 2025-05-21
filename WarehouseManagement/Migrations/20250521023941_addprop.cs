using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseManagement.Migrations
{
    /// <inheritdoc />
    public partial class addprop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseDetails_Warehouses_WarehouseId",
                table: "WarehouseDetails");

            migrationBuilder.DropIndex(
                name: "IX_WarehouseDetails_WarehouseId",
                table: "WarehouseDetails");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "WarehouseDetails");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ExportDetails",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseDetails_WareId",
                table: "WarehouseDetails",
                column: "WareId");

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseDetails_Warehouses_WareId",
                table: "WarehouseDetails",
                column: "WareId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseDetails_Warehouses_WareId",
                table: "WarehouseDetails");

            migrationBuilder.DropIndex(
                name: "IX_WarehouseDetails_WareId",
                table: "WarehouseDetails");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ExportDetails");

            migrationBuilder.AddColumn<int>(
                name: "WarehouseId",
                table: "WarehouseDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Products",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseDetails_WarehouseId",
                table: "WarehouseDetails",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseDetails_Warehouses_WarehouseId",
                table: "WarehouseDetails",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id");
        }
    }
}
