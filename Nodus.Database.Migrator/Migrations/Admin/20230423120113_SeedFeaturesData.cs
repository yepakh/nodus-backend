using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Nodus.Database.Migrator.Migrations.Admin
{
    /// <inheritdoc />
    public partial class SeedFeaturesData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Features",
                columns: new[] { "ID", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Feature for super admin only", "Manage companies" },
                    { 2, "Feature for company admins", "Manage users" },
                    { 3, "Feature for trips managers", "Manage trips" },
                    { 4, "Feature for accountants", "Manage bills" },
                    { 5, "Additional feature", "Access statistics" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Features",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Features",
                keyColumn: "ID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Features",
                keyColumn: "ID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Features",
                keyColumn: "ID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Features",
                keyColumn: "ID",
                keyValue: 5);
        }
    }
}
