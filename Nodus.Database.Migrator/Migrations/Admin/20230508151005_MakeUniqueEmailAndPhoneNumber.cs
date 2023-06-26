using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nodus.Database.Migrator.Migrations.Admin
{
    public partial class MakeUniqueEmailAndPhoneNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Companies_CompanyID",
                table: "Roles");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Roles",
                type: "varchar(1024)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1024)");

            migrationBuilder.AlterColumn<int>(
                name: "CompanyID",
                table: "Roles",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Features",
                type: "varchar(1024)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1024)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Companies",
                type: "varchar(1024)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1024)");

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "ID",
                keyValue: 1,
                column: "Name",
                value: "ManageCompanies");

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "ID",
                keyValue: 2,
                column: "Name",
                value: "ManageUsers");

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "ID",
                keyValue: 3,
                column: "Name",
                value: "ManageTrips");

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "ID",
                keyValue: 4,
                column: "Name",
                value: "ManageBills");

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "ID",
                keyValue: 5,
                column: "Name",
                value: "AccessStatistics");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_PhoneNumber",
                table: "Users",
                column: "PhoneNumber",
                unique: true,
                filter: "[PhoneNumber] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Companies_CompanyID",
                table: "Roles",
                column: "CompanyID",
                principalTable: "Companies",
                principalColumn: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Companies_CompanyID",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_PhoneNumber",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Roles",
                type: "varchar(1024)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(1024)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CompanyID",
                table: "Roles",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Features",
                type: "varchar(1024)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(1024)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Companies",
                type: "varchar(1024)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(1024)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "ID",
                keyValue: 1,
                column: "Name",
                value: "Manage companies");

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "ID",
                keyValue: 2,
                column: "Name",
                value: "Manage users");

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "ID",
                keyValue: 3,
                column: "Name",
                value: "Manage trips");

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "ID",
                keyValue: 4,
                column: "Name",
                value: "Manage bills");

            migrationBuilder.UpdateData(
                table: "Features",
                keyColumn: "ID",
                keyValue: 5,
                column: "Name",
                value: "Access statistics");

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Companies_CompanyID",
                table: "Roles",
                column: "CompanyID",
                principalTable: "Companies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
