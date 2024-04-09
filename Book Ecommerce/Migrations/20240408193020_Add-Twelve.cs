using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book_Ecommerce.Migrations
{
    public partial class AddTwelve : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Provinces",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    FullNameEn = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    CodeName = table.Column<string>(type: "nvarchar(255)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provinces", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Districts",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    FullNameEn = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    CodeName = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    ProvinceCode = table.Column<string>(type: "nvarchar(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Districts", x => x.Code);
                    table.ForeignKey(
                        name: "FK_Districts_Provinces_ProvinceCode",
                        column: x => x.ProvinceCode,
                        principalTable: "Provinces",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wards",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    FullNameEn = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    CodeName = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    DistrictCode = table.Column<string>(type: "nvarchar(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wards", x => x.Code);
                    table.ForeignKey(
                        name: "FK_Wards_Districts_DistrictCode",
                        column: x => x.DistrictCode,
                        principalTable: "Districts",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Districts_ProvinceCode",
                table: "Districts",
                column: "ProvinceCode");

            migrationBuilder.CreateIndex(
                name: "IX_Wards_DistrictCode",
                table: "Wards",
                column: "DistrictCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Wards");

            migrationBuilder.DropTable(
                name: "Districts");

            migrationBuilder.DropTable(
                name: "Provinces");
        }
    }
}
