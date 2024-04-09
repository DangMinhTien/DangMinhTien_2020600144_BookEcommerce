using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book_Ecommerce.Migrations
{
    public partial class AddTen : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Favourites");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "MaxCodeNumber",
                table: "Products",
                newName: "CodeNumber");

            migrationBuilder.RenameColumn(
                name: "MaxCodeNumber",
                table: "Orders",
                newName: "CodeNumber");

            migrationBuilder.RenameColumn(
                name: "MaxCodeNumber",
                table: "Customers",
                newName: "CodeNumber");

            migrationBuilder.RenameColumn(
                name: "MaxCodeNumber",
                table: "Categories",
                newName: "CodeNumber");

            migrationBuilder.RenameColumn(
                name: "MaxCodeNumber",
                table: "Brands",
                newName: "CodeNumber");

            migrationBuilder.RenameColumn(
                name: "Decription",
                table: "Banners",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "MaxCodeNumber",
                table: "Authors",
                newName: "CodeNumber");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Orders",
                type: "nvarchar(250)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "CodeNumber",
                table: "Employees",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "Cart",
                columns: table => new
                {
                    CustomerId = table.Column<string>(type: "char(36)", nullable: false),
                    ProductId = table.Column<string>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cart", x => new { x.CustomerId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_Cart_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Cart_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FavouriteProducts",
                columns: table => new
                {
                    CustomerId = table.Column<string>(type: "char(36)", nullable: false),
                    ProductId = table.Column<string>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavouriteProducts", x => new { x.CustomerId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_FavouriteProducts_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FavouriteProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CodeNumber",
                table: "Products",
                column: "CodeNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CodeNumber",
                table: "Orders",
                column: "CodeNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_CodeNumber",
                table: "Employees",
                column: "CodeNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_CodeNumber",
                table: "Customers",
                column: "CodeNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CodeNumber",
                table: "Categories",
                column: "CodeNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Brands_CodeNumber",
                table: "Brands",
                column: "CodeNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Authors_CodeNumber",
                table: "Authors",
                column: "CodeNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cart_ProductId",
                table: "Cart",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_FavouriteProducts_ProductId",
                table: "FavouriteProducts",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cart");

            migrationBuilder.DropTable(
                name: "FavouriteProducts");

            migrationBuilder.DropIndex(
                name: "IX_Products_CodeNumber",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CodeNumber",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Employees_CodeNumber",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Customers_CodeNumber",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Categories_CodeNumber",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Brands_CodeNumber",
                table: "Brands");

            migrationBuilder.DropIndex(
                name: "IX_Authors_CodeNumber",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CodeNumber",
                table: "Employees");

            migrationBuilder.RenameColumn(
                name: "CodeNumber",
                table: "Products",
                newName: "MaxCodeNumber");

            migrationBuilder.RenameColumn(
                name: "CodeNumber",
                table: "Orders",
                newName: "MaxCodeNumber");

            migrationBuilder.RenameColumn(
                name: "CodeNumber",
                table: "Customers",
                newName: "MaxCodeNumber");

            migrationBuilder.RenameColumn(
                name: "CodeNumber",
                table: "Categories",
                newName: "MaxCodeNumber");

            migrationBuilder.RenameColumn(
                name: "CodeNumber",
                table: "Brands",
                newName: "MaxCodeNumber");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Banners",
                newName: "Decription");

            migrationBuilder.RenameColumn(
                name: "CodeNumber",
                table: "Authors",
                newName: "MaxCodeNumber");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Favourites",
                columns: table => new
                {
                    CustomerId = table.Column<string>(type: "char(36)", nullable: false),
                    ProductId = table.Column<string>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favourites", x => new { x.CustomerId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_Favourites_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Favourites_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Favourites_ProductId",
                table: "Favourites",
                column: "ProductId");
        }
    }
}
