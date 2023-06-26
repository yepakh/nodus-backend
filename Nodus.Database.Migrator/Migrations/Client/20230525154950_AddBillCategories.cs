using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nodus.Database.Migrator.Migrations.Client
{
    public partial class AddBillCategories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BillCategoryID",
                table: "Bills",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BillCategories",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillCategories", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bills_BillCategoryID",
                table: "Bills",
                column: "BillCategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_Bills_BillCategories_BillCategoryID",
                table: "Bills",
                column: "BillCategoryID",
                principalTable: "BillCategories",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bills_BillCategories_BillCategoryID",
                table: "Bills");

            migrationBuilder.DropTable(
                name: "BillCategories");

            migrationBuilder.DropIndex(
                name: "IX_Bills_BillCategoryID",
                table: "Bills");

            migrationBuilder.DropColumn(
                name: "BillCategoryID",
                table: "Bills");
        }
    }
}
