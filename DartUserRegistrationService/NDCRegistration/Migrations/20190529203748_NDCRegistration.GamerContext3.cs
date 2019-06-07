using Microsoft.EntityFrameworkCore.Migrations;

namespace NDCRegistration.Migrations
{
    public partial class NDCRegistrationGamerContext3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Game_Gamers_GamerId",
                table: "Game");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Game",
                table: "Game");

            migrationBuilder.RenameTable(
                name: "Game",
                newName: "Games");

            migrationBuilder.RenameIndex(
                name: "IX_Game_GamerId",
                table: "Games",
                newName: "IX_Games_GamerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Games",
                table: "Games",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Gamers_GamerId",
                table: "Games",
                column: "GamerId",
                principalTable: "Gamers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_Gamers_GamerId",
                table: "Games");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Games",
                table: "Games");

            migrationBuilder.RenameTable(
                name: "Games",
                newName: "Game");

            migrationBuilder.RenameIndex(
                name: "IX_Games_GamerId",
                table: "Game",
                newName: "IX_Game_GamerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Game",
                table: "Game",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Game_Gamers_GamerId",
                table: "Game",
                column: "GamerId",
                principalTable: "Gamers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
