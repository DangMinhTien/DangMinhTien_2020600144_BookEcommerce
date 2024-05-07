using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book_Ecommerce.Data.Migrations
{
    public partial class AddEight : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthorProducts_Authors_AthorId",
                table: "AuthorProducts");

            migrationBuilder.RenameColumn(
                name: "AthorId",
                table: "AuthorProducts",
                newName: "AuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_AuthorProducts_AthorId",
                table: "AuthorProducts",
                newName: "IX_AuthorProducts_AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorProducts_Authors_AuthorId",
                table: "AuthorProducts",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "AuthorId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthorProducts_Authors_AuthorId",
                table: "AuthorProducts");

            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "AuthorProducts",
                newName: "AthorId");

            migrationBuilder.RenameIndex(
                name: "IX_AuthorProducts_AuthorId",
                table: "AuthorProducts",
                newName: "IX_AuthorProducts_AthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorProducts_Authors_AthorId",
                table: "AuthorProducts",
                column: "AthorId",
                principalTable: "Authors",
                principalColumn: "AuthorId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
