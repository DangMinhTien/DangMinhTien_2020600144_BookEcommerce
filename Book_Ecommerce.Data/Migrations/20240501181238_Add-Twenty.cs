using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book_Ecommerce.Data.Migrations
{
    public partial class AddTwenty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Decription",
                table: "Products",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "Image",
                table: "Brands",
                newName: "ImageName");

            migrationBuilder.RenameColumn(
                name: "Image",
                table: "Banners",
                newName: "ImageName");

            migrationBuilder.AddColumn<string>(
                name: "UrlImage",
                table: "Brands",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UrlImage",
                table: "Banners",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UrlImage",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "UrlImage",
                table: "Banners");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Products",
                newName: "Decription");

            migrationBuilder.RenameColumn(
                name: "ImageName",
                table: "Brands",
                newName: "Image");

            migrationBuilder.RenameColumn(
                name: "ImageName",
                table: "Banners",
                newName: "Image");
        }
    }
}
