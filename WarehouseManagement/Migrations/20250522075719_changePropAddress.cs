using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseManagement.Migrations
{
    /// <inheritdoc />
    public partial class changePropAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImportDetails_Imports_ImportId",
                table: "ImportDetails");

            migrationBuilder.DropIndex(
                name: "IX_ImportDetails_ImportId",
                table: "ImportDetails");

            migrationBuilder.DropColumn(
                name: "ImportId",
                table: "ImportDetails");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Exports",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImportDetails_ImpId",
                table: "ImportDetails",
                column: "ImpId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImportDetails_Imports_ImpId",
                table: "ImportDetails",
                column: "ImpId",
                principalTable: "Imports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImportDetails_Imports_ImpId",
                table: "ImportDetails");

            migrationBuilder.DropIndex(
                name: "IX_ImportDetails_ImpId",
                table: "ImportDetails");

            migrationBuilder.AddColumn<int>(
                name: "ImportId",
                table: "ImportDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Address",
                table: "Exports",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImportDetails_ImportId",
                table: "ImportDetails",
                column: "ImportId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImportDetails_Imports_ImportId",
                table: "ImportDetails",
                column: "ImportId",
                principalTable: "Imports",
                principalColumn: "Id");
        }
    }
}
