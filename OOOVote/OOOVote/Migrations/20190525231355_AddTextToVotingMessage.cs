using Microsoft.EntityFrameworkCore.Migrations;

namespace OOOVote.Migrations
{
    public partial class AddTextToVotingMessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "VotingMessages",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Message",
                table: "VotingMessages");
        }
    }
}
