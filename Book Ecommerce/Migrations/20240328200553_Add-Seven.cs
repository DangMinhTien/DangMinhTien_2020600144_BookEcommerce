using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book_Ecommerce.Migrations
{
    public partial class AddSeven : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    AuthorId = table.Column<string>(type: "char(36)", nullable: false),
                    AuthorCode = table.Column<string>(type: "varchar(250)", nullable: false),
                    MaxCodeNumber = table.Column<long>(type: "bigint", nullable: false),
                    AuthorName = table.Column<string>(type: "nvarchar(250)", nullable: false),
                    AuthorSlug = table.Column<string>(type: "varchar(250)", nullable: false),
                    Information = table.Column<string>(type: "varchar(250)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.AuthorId);
                });

            migrationBuilder.CreateTable(
                name: "AuthorProducts",
                columns: table => new
                {
                    AthorId = table.Column<string>(type: "char(36)", nullable: false),
                    ProductId = table.Column<string>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorProducts", x => new { x.ProductId, x.AthorId });
                    table.ForeignKey(
                        name: "FK_AuthorProducts_Authors_AthorId",
                        column: x => x.AthorId,
                        principalTable: "Authors",
                        principalColumn: "AuthorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuthorProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductName",
                table: "Products",
                column: "ProductName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CategoryName",
                table: "Categories",
                column: "CategoryName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Brands_BrandName",
                table: "Brands",
                column: "BrandName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuthorProducts_AthorId",
                table: "AuthorProducts",
                column: "AthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Authors_AuthorCode",
                table: "Authors",
                column: "AuthorCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Authors_AuthorName",
                table: "Authors",
                column: "AuthorName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Authors_AuthorSlug",
                table: "Authors",
                column: "AuthorSlug",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthorProducts");

            migrationBuilder.DropTable(
                name: "Authors");

            migrationBuilder.DropIndex(
                name: "IX_Products_ProductName",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Categories_CategoryName",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Brands_BrandName",
                table: "Brands");
        }
    }
}
