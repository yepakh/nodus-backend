using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nodus.Database.Migrator.Migrations.Admin
{
    public partial class ChangeTripRoleSystem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("1b3c265a-f9e1-4764-ba03-d3e93699c00f"));

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "ID",
                keyValue: 2,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Allows to read users personal information", "ReadUsers" });

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "ID",
                keyValue: 3,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Allows to update users personal information", "WriteUsers" });

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "ID",
                keyValue: 4,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Allows to access trips", "ReadTrips" });

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "ID",
                keyValue: 5,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Allows to manage trips", "WriteTrips" });

            migrationBuilder.InsertData(
                table: "Features",
                columns: new[] { "ID", "Description", "Name" },
                values: new object[,]
                {
                    { 6, "Allows to access bills", "ReadBills" },
                    { 7, "Allows to create bills", "WriteBills" },
                    { 8, "Additional feature", "AccessStatistics" }
                });

            migrationBuilder.InsertData(
                table: "RoleFeatures",
                columns: new[] { "FeatureID", "RoleID" },
                values: new object[] { 1, -1 });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Created", "Email", "IsActive", "LastLogin", "PasswordHash", "PasswordSalt", "PhoneNumber", "RoleID" },
                values: new object[] { new Guid("acb93144-58b2-48e2-a4e8-e6a6fb138f06"), new DateTime(2023, 5, 25, 13, 28, 7, 945, DateTimeKind.Local).AddTicks(404), "nodus.admin@email.com", true, null, new byte[] { 191, 62, 228, 119, 85, 50, 10, 68, 224, 66, 237, 157, 108, 153, 19, 10, 120, 232, 154, 217, 48, 109, 118, 176, 179, 23, 34, 5, 20, 125, 36, 190 }, new byte[] { 58, 138, 81, 250, 144, 47, 111, 56, 113, 130, 18, 245, 7, 223, 241, 254, 160, 137, 187, 235, 233, 233, 198, 47, 169, 98, 155, 144, 222, 150, 22, 111 }, "1234567890", -1 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Features",
                keyColumn: "ID",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Features",
                keyColumn: "ID",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Features",
                keyColumn: "ID",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "RoleFeatures",
                keyColumns: new[] { "FeatureID", "RoleID" },
                keyValues: new object[] { 1, -1 });

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("acb93144-58b2-48e2-a4e8-e6a6fb138f06"));

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "ID",
                keyValue: 2,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Feature for company admins", "ManageUsers" });

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "ID",
                keyValue: 3,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Feature for trips managers", "ManageTrips" });

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "ID",
                keyValue: 4,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Feature for accountants", "ManageBills" });

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "ID",
                keyValue: 5,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Additional feature", "AccessStatistics" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Created", "Email", "IsActive", "LastLogin", "PasswordHash", "PasswordSalt", "PhoneNumber", "RoleID" },
                values: new object[] { new Guid("1b3c265a-f9e1-4764-ba03-d3e93699c00f"), new DateTime(2023, 5, 22, 16, 6, 35, 394, DateTimeKind.Local).AddTicks(3068), "nodus.admin@email.com", true, null, new byte[] { 59, 45, 182, 185, 17, 55, 91, 64, 180, 232, 255, 25, 235, 92, 205, 82, 104, 189, 101, 3, 73, 191, 5, 210, 113, 51, 224, 44, 79, 60, 97, 6 }, new byte[] { 183, 29, 135, 171, 162, 238, 173, 104, 170, 221, 23, 141, 152, 40, 60, 60, 87, 183, 170, 41, 87, 214, 25, 41, 169, 56, 172, 149, 128, 34, 31, 191 }, "1234567890", -1 });
        }
    }
}
