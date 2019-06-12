using Microsoft.EntityFrameworkCore.Migrations;

namespace NDCRegistration.Migrations
{
    public partial class RemovedGDPRFieldsFromUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Gamers");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Gamers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Gamers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Gamers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Gamers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Gamers",
                nullable: true);
        }
    }
}
