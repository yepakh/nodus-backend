using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nodus.Database.Migrator.Migrations.Admin
{
    public partial class SeedSuperAdminUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "ID", "CompanyID", "Description", "Name" },
                values: new object[] { -1, null, "Super admin role for admin-side workers", "SuperAdmin" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Created", "Email", "IsActive", "LastLogin", "PasswordHash", "PasswordSalt", "PhoneNumber", "RoleID" },
                values: new object[] { new Guid("0311ddca-0795-4606-b8d0-8d3c46a734fd"), new DateTime(2023, 5, 15, 18, 35, 37, 652, DateTimeKind.Local).AddTicks(5958), "nodus.admin@email.com", true, null, new byte[] { 86, 252, 23, 141, 56, 250, 53, 58, 32, 128, 70, 90, 14, 4, 241, 28, 71, 125, 199, 192, 11, 4, 204, 76, 13, 250, 252, 24, 23, 101, 101, 8 }, new byte[] { 245, 216, 246, 71, 1, 249, 224, 31, 238, 155, 224, 87, 51, 209, 99, 116, 111, 153, 114, 124, 152, 242, 1, 134, 134, 34, 144, 130, 239, 243, 127, 215 }, "1234567890", -1 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("0311ddca-0795-4606-b8d0-8d3c46a734fd"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "ID",
                keyValue: -1);
        }
    }
}
