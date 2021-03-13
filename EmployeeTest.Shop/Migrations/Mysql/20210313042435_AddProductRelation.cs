using Microsoft.EntityFrameworkCore.Migrations;

namespace OliDemos.Shop.Migrations.Mysql
{
    public partial class AddProductRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "UserId",
                table: "Product",
                type: "bigint unsigned",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.CreateIndex(
                name: "IX_Product_UserId",
                table: "Product",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_AspNetUsers_UserId",
                table: "Product",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_AspNetUsers_UserId",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_UserId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Product");
        }
    }
}
