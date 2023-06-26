using System;
using System.Security.Principal;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nodus.Database.Migrator.Migrations.Client
{
    public partial class ChangeDocIdToGuid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
               name: "PK_Documents",
               table: "Documents");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "Documents");

            migrationBuilder.AddColumn<Guid>(
                name: "ID",
                table: "Documents",
                type: "uniqueidentifier",
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Documents",
                table: "Documents",
                column: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Documents",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "Documents");

            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "Documents",
                type: "int",
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Documents",
                table: "Documents",
                column: "ID");
        }
    }
}
