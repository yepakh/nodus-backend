using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nodus.Database.Migrator.Migrations.Client
{
    public partial class ChangedBudgetForTripsAndUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxBudget",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "MinBudget",
                table: "Trips");

            migrationBuilder.AddColumn<double>(
                name: "Budget",
                table: "UserTrips",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Budget",
                table: "Trips",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Budget",
                table: "UserTrips");

            migrationBuilder.DropColumn(
                name: "Budget",
                table: "Trips");

            migrationBuilder.AddColumn<double>(
                name: "MaxBudget",
                table: "Trips",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MinBudget",
                table: "Trips",
                type: "float",
                nullable: true);
        }
    }
}
