using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book_Ecommerce.Data.Migrations
{
    public partial class AddTwentyTwo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Decription",
                table: "Categories",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "Decription",
                table: "Brands",
                newName: "Description");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Categories",
                newName: "Decription");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Brands",
                newName: "Decription");
        }
    }
}
