using Microsoft.EntityFrameworkCore.Migrations;

namespace Forum_Dyskusyjne.Migrations
{
    public partial class WidocznoscPrivMessages2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorVisible",
                table: "PrivateMessages");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AuthorVisible",
                table: "PrivateMessages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
