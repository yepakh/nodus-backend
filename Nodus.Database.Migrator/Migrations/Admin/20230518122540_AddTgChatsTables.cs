using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nodus.Database.Migrator.Migrations.Admin
{
    public partial class AddTgChatsTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TgChats",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TgChats", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TgChats_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MessagesWithInlineItems",
                columns: table => new
                {
                    MessageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChatID = table.Column<long>(type: "bigint", nullable: false),
                    DateSent = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MessageType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessagesWithInlineItems", x => x.MessageID);
                    table.ForeignKey(
                        name: "FK_MessagesWithInlineItems_TgChats_ChatID",
                        column: x => x.ChatID,
                        principalTable: "TgChats",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("0311ddca-0795-4606-b8d0-8d3c46a734fd"),
                columns: new[] { "Created", "PasswordHash", "PasswordSalt" },
                values: new object[] { new DateTime(2023, 5, 18, 15, 25, 40, 170, DateTimeKind.Local).AddTicks(131), new byte[] { 28, 220, 136, 239, 232, 232, 124, 146, 87, 153, 251, 72, 74, 138, 112, 161, 179, 96, 65, 84, 38, 119, 140, 139, 17, 221, 236, 82, 174, 120, 82, 144 }, new byte[] { 185, 175, 188, 192, 42, 144, 6, 220, 88, 122, 247, 66, 36, 10, 6, 123, 165, 8, 99, 89, 211, 44, 164, 219, 94, 42, 163, 56, 26, 121, 92, 118 } });

            migrationBuilder.CreateIndex(
                name: "IX_MessagesWithInlineItems_ChatID",
                table: "MessagesWithInlineItems",
                column: "ChatID");

            migrationBuilder.CreateIndex(
                name: "IX_TgChats_UserID",
                table: "TgChats",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessagesWithInlineItems");

            migrationBuilder.DropTable(
                name: "TgChats");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("0311ddca-0795-4606-b8d0-8d3c46a734fd"),
                columns: new[] { "Created", "PasswordHash", "PasswordSalt" },
                values: new object[] { new DateTime(2023, 5, 15, 18, 35, 37, 652, DateTimeKind.Local).AddTicks(5958), new byte[] { 86, 252, 23, 141, 56, 250, 53, 58, 32, 128, 70, 90, 14, 4, 241, 28, 71, 125, 199, 192, 11, 4, 204, 76, 13, 250, 252, 24, 23, 101, 101, 8 }, new byte[] { 245, 216, 246, 71, 1, 249, 224, 31, 238, 155, 224, 87, 51, 209, 99, 116, 111, 153, 114, 124, 152, 242, 1, 134, 134, 34, 144, 130, 239, 243, 127, 215 } });
        }
    }
}
