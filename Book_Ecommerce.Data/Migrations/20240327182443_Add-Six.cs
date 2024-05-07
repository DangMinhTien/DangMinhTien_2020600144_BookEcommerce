using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book_Ecommerce.Data.Migrations
{
    public partial class AddSix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_CategoryId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "BannerName",
                table: "Banners",
                newName: "Title");

            migrationBuilder.AddColumn<string>(
                name: "Decription",
                table: "Banners",
                type: "nvarchar(500)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "CategoryProdcuts",
                columns: table => new
                {
                    CategoryId = table.Column<string>(type: "char(36)", nullable: false),
                    ProductId = table.Column<string>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryProdcuts", x => new { x.ProductId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_CategoryProdcuts_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryProdcuts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryProdcuts_CategoryId",
                table: "CategoryProdcuts",
                column: "CategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryProdcuts");

            migrationBuilder.DropColumn(
                name: "Decription",
                table: "Banners");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Banners",
                newName: "BannerName");

            migrationBuilder.AddColumn<string>(
                name: "CategoryId",
                table: "Products",
                type: "char(36)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
