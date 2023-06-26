using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nodus.Database.Migrator.Migrations.Client
{
    public partial class AddedDocsToHistBills : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DocumentsInHistoricalBills",
                columns: table => new
                {
                    DocumentsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HistoricalBillsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentsInHistoricalBills", x => new { x.DocumentsId, x.HistoricalBillsId });
                    table.ForeignKey(
                        name: "FK_DocumentsInHistoricalBills_Documents_DocumentsId",
                        column: x => x.DocumentsId,
                        principalTable: "Documents",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentsInHistoricalBills_HistoricalBills_HistoricalBillsId",
                        column: x => x.HistoricalBillsId,
                        principalTable: "HistoricalBills",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentsInHistoricalBills_HistoricalBillsId",
                table: "DocumentsInHistoricalBills",
                column: "HistoricalBillsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentsInHistoricalBills");
        }
    }
}
