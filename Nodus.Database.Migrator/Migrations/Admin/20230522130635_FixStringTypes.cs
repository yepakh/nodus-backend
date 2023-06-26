using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nodus.Database.Migrator.Migrations.Admin
{
    public partial class FixStringTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("0311ddca-0795-4606-b8d0-8d3c46a734fd"));

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(256)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(256)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Roles",
                type: "nvarchar(64)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(64)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Roles",
                type: "nvarchar(1024)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1024)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Features",
                type: "nvarchar(64)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(64)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Features",
                type: "nvarchar(1024)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1024)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Companies",
                type: "nvarchar(64)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(64)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Companies",
                type: "nvarchar(1024)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1024)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ConnectionString",
                table: "Companies",
                type: "nvarchar(512)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(512)");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Created", "Email", "IsActive", "LastLogin", "PasswordHash", "PasswordSalt", "PhoneNumber", "RoleID" },
                values: new object[] { new Guid("1b3c265a-f9e1-4764-ba03-d3e93699c00f"), new DateTime(2023, 5, 22, 16, 6, 35, 394, DateTimeKind.Local).AddTicks(3068), "nodus.admin@email.com", true, null, new byte[] { 59, 45, 182, 185, 17, 55, 91, 64, 180, 232, 255, 25, 235, 92, 205, 82, 104, 189, 101, 3, 73, 191, 5, 210, 113, 51, 224, 44, 79, 60, 97, 6 }, new byte[] { 183, 29, 135, 171, 162, 238, 173, 104, 170, 221, 23, 141, 152, 40, 60, 60, 87, 183, 170, 41, 87, 214, 25, 41, 169, 56, 172, 149, 128, 34, 31, 191 }, "1234567890", -1 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("1b3c265a-f9e1-4764-ba03-d3e93699c00f"));

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "varchar(256)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Roles",
                type: "varchar(64)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Roles",
                type: "varchar(1024)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1024)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Features",
                type: "varchar(64)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Features",
                type: "varchar(1024)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1024)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Companies",
                type: "varchar(64)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Companies",
                type: "varchar(1024)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1024)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ConnectionString",
                table: "Companies",
                type: "varchar(512)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(512)");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Created", "Email", "IsActive", "LastLogin", "PasswordHash", "PasswordSalt", "PhoneNumber", "RoleID" },
                values: new object[] { new Guid("0311ddca-0795-4606-b8d0-8d3c46a734fd"), new DateTime(2023, 5, 15, 18, 35, 37, 652, DateTimeKind.Local).AddTicks(5958), "nodus.admin@email.com", true, null, new byte[] { 86, 252, 23, 141, 56, 250, 53, 58, 32, 128, 70, 90, 14, 4, 241, 28, 71, 125, 199, 192, 11, 4, 204, 76, 13, 250, 252, 24, 23, 101, 101, 8 }, new byte[] { 245, 216, 246, 71, 1, 249, 224, 31, 238, 155, 224, 87, 51, 209, 99, 116, 111, 153, 114, 124, 152, 242, 1, 134, 134, 34, 144, 130, 239, 243, 127, 215 }, "1234567890", -1 });
        }
    }
}
