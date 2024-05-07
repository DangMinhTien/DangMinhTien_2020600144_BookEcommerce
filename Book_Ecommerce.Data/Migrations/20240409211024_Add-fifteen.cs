using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book_Ecommerce.Data.Migrations
{
    public partial class Addfifteen : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryProdcuts_Categories_CategoryId",
                table: "CategoryProdcuts");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryProdcuts_Products_ProductId",
                table: "CategoryProdcuts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoryProdcuts",
                table: "CategoryProdcuts");

            migrationBuilder.RenameTable(
                name: "CategoryProdcuts",
                newName: "CategoryProducts");

            migrationBuilder.RenameIndex(
                name: "IX_CategoryProdcuts_CategoryId",
                table: "CategoryProducts",
                newName: "IX_CategoryProducts_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoryProducts",
                table: "CategoryProducts",
                columns: new[] { "ProductId", "CategoryId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryProducts_Categories_CategoryId",
                table: "CategoryProducts",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryProducts_Products_ProductId",
                table: "CategoryProducts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryProducts_Categories_CategoryId",
                table: "CategoryProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryProducts_Products_ProductId",
                table: "CategoryProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoryProducts",
                table: "CategoryProducts");

            migrationBuilder.RenameTable(
                name: "CategoryProducts",
                newName: "CategoryProdcuts");

            migrationBuilder.RenameIndex(
                name: "IX_CategoryProducts_CategoryId",
                table: "CategoryProdcuts",
                newName: "IX_CategoryProdcuts_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoryProdcuts",
                table: "CategoryProdcuts",
                columns: new[] { "ProductId", "CategoryId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryProdcuts_Categories_CategoryId",
                table: "CategoryProdcuts",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryProdcuts_Products_ProductId",
                table: "CategoryProdcuts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
