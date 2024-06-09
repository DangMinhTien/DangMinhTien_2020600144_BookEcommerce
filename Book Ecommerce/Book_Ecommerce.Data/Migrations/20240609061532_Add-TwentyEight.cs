using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book_Ecommerce.Data.Migrations
{
    public partial class AddTwentyEight : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Messsages",
                columns: table => new
                {
                    MessageId = table.Column<string>(type: "char(36)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(4000)", nullable: false),
                    SendDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SendBy = table.Column<string>(type: "char(36)", nullable: false),
                    CustomerId = table.Column<string>(type: "char(36)", nullable: false),
                    EmployeeId = table.Column<string>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messsages", x => x.MessageId);
                    table.ForeignKey(
                        name: "FK_Messsages_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Messsages_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messsages_CustomerId",
                table: "Messsages",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Messsages_EmployeeId",
                table: "Messsages",
                column: "EmployeeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messsages");
        }
    }
}
