using Microsoft.EntityFrameworkCore.Migrations;

namespace Forum_Dyskusyjne.Migrations
{
    public partial class WidocznoscPrivMessages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AuthorVisible",
                table: "PrivateMessages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ReceiverVisible",
                table: "PrivateMessages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorVisible",
                table: "PrivateMessages");

            migrationBuilder.DropColumn(
                name: "ReceiverVisible",
                table: "PrivateMessages");
        }
    }
}
