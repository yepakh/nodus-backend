using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nodus.Database.Migrator.Migrations.Client
{
    public partial class SeededBillSattuses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "BillStatuses",
                columns: new[] { "ID", "Name" },
                values: new object[] { 1, "New" });

            migrationBuilder.InsertData(
                table: "BillStatuses",
                columns: new[] { "ID", "Name" },
                values: new object[] { 2, "Active" });

            migrationBuilder.InsertData(
                table: "BillStatuses",
                columns: new[] { "ID", "Name" },
                values: new object[] { 3, "Inactive" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BillStatuses",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "BillStatuses",
                keyColumn: "ID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "BillStatuses",
                keyColumn: "ID",
                keyValue: 3);
        }
    }
}
