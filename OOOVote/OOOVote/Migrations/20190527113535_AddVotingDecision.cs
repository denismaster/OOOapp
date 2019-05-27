using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OOOVote.Migrations
{
    public partial class AddVotingDecision : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VotingDecisions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Decision = table.Column<int>(nullable: false),
                    VotingOptionId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VotingDecisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VotingDecisions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VotingDecisions_VotingOptions_VotingOptionId",
                        column: x => x.VotingOptionId,
                        principalTable: "VotingOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VotingDecisions_UserId",
                table: "VotingDecisions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VotingDecisions_VotingOptionId",
                table: "VotingDecisions",
                column: "VotingOptionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VotingDecisions");
        }
    }
}
