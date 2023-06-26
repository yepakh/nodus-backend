using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nodus.Database.Migrations.Migrations.Client
{
    public partial class AddBaseTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BillStatuses",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(64)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillStatuses", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TripRoles",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(64)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripRoles", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "UserDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "varchar(256)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "varchar(64)", nullable: false),
                    LastName = table.Column<string>(type: "varchar(64)", nullable: false),
                    Address = table.Column<string>(type: "varchar(256)", nullable: false),
                    Notes = table.Column<string>(type: "varchar(1024)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Trips",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(64)", nullable: false),
                    Description = table.Column<string>(type: "varchar(1024)", nullable: false),
                    MinBudget = table.Column<double>(type: "float", nullable: true),
                    MaxBudget = table.Column<double>(type: "float", nullable: true),
                    CreatorID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateTimeCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartOfTrip = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndOfTrip = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trips", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Trips_UserDetails_CreatorID",
                        column: x => x.CreatorID,
                        principalTable: "UserDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Bills",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TripId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(64)", nullable: false),
                    Desciption = table.Column<string>(type: "varchar(1024)", nullable: false),
                    Summary = table.Column<double>(type: "float", nullable: true),
                    DateTimeCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateTimeEdited = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EditorID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatusID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bills", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Bills_BillStatuses_StatusID",
                        column: x => x.StatusID,
                        principalTable: "BillStatuses",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bills_Trips_TripId",
                        column: x => x.TripId,
                        principalTable: "Trips",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bills_UserDetails_CreatorID",
                        column: x => x.CreatorID,
                        principalTable: "UserDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bills_UserDetails_EditorID",
                        column: x => x.EditorID,
                        principalTable: "UserDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserTrips",
                columns: table => new
                {
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TripID = table.Column<int>(type: "int", nullable: false),
                    TripRoleID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTrips", x => new { x.TripID, x.UserID });
                    table.ForeignKey(
                        name: "FK_UserTrips_TripRoles_TripRoleID",
                        column: x => x.TripRoleID,
                        principalTable: "TripRoles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTrips_Trips_TripID",
                        column: x => x.TripID,
                        principalTable: "Trips",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTrips_UserDetails_UserID",
                        column: x => x.UserID,
                        principalTable: "UserDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Link = table.Column<string>(type: "varchar(1024)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DateTimeCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeactivatorID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BillID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Documents_Bills_BillID",
                        column: x => x.BillID,
                        principalTable: "Bills",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Documents_UserDetails_CreatorID",
                        column: x => x.CreatorID,
                        principalTable: "UserDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Documents_UserDetails_DeactivatorID",
                        column: x => x.DeactivatorID,
                        principalTable: "UserDetails",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HistoricalBills",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BillID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(64)", nullable: false),
                    Desciption = table.Column<string>(type: "varchar(1024)", nullable: false),
                    Summary = table.Column<double>(type: "float", nullable: true),
                    DateTimeEdit = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditorID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatusID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricalBills", x => x.ID);
                    table.ForeignKey(
                        name: "FK_HistoricalBills_Bills_BillID",
                        column: x => x.BillID,
                        principalTable: "Bills",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HistoricalBills_BillStatuses_StatusID",
                        column: x => x.StatusID,
                        principalTable: "BillStatuses",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistoricalBills_UserDetails_EditorID",
                        column: x => x.EditorID,
                        principalTable: "UserDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bills_CreatorID",
                table: "Bills",
                column: "CreatorID");

            migrationBuilder.CreateIndex(
                name: "IX_Bills_EditorID",
                table: "Bills",
                column: "EditorID");

            migrationBuilder.CreateIndex(
                name: "IX_Bills_StatusID",
                table: "Bills",
                column: "StatusID");

            migrationBuilder.CreateIndex(
                name: "IX_Bills_TripId",
                table: "Bills",
                column: "TripId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_BillID",
                table: "Documents",
                column: "BillID");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_CreatorID",
                table: "Documents",
                column: "CreatorID");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_DeactivatorID",
                table: "Documents",
                column: "DeactivatorID");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricalBills_BillID",
                table: "HistoricalBills",
                column: "BillID");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricalBills_EditorID",
                table: "HistoricalBills",
                column: "EditorID");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricalBills_StatusID",
                table: "HistoricalBills",
                column: "StatusID");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_CreatorID",
                table: "Trips",
                column: "CreatorID");

            migrationBuilder.CreateIndex(
                name: "IX_UserTrips_TripRoleID",
                table: "UserTrips",
                column: "TripRoleID");

            migrationBuilder.CreateIndex(
                name: "IX_UserTrips_UserID",
                table: "UserTrips",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "HistoricalBills");

            migrationBuilder.DropTable(
                name: "UserTrips");

            migrationBuilder.DropTable(
                name: "Bills");

            migrationBuilder.DropTable(
                name: "TripRoles");

            migrationBuilder.DropTable(
                name: "BillStatuses");

            migrationBuilder.DropTable(
                name: "Trips");

            migrationBuilder.DropTable(
                name: "UserDetails");
        }
    }
}
