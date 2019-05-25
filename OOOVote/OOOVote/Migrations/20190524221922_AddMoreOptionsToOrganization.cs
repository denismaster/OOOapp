using Microsoft.EntityFrameworkCore.Migrations;

namespace OOOVote.Migrations
{
    public partial class AddMoreOptionsToOrganization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RuleType",
                table: "Organizations",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RulesUrl",
                table: "Organizations",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ShareСapital",
                table: "Organizations",
                nullable: false,
                defaultValue: 10000m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RuleType",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "RulesUrl",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "ShareСapital",
                table: "Organizations");
        }
    }
}
