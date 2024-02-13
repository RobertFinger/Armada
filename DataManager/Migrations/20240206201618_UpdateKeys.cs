using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataManager.Migrations
{
    /// <inheritdoc />
    public partial class UpdateKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Rounds",
                table: "Games",
                newName: "MinLetters");

            migrationBuilder.RenameColumn(
                name: "CorrectAnswer",
                table: "Games",
                newName: "MaxLetters");

            migrationBuilder.AddColumn<int>(
                name: "Guesses",
                table: "Games",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ChallengeWord",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlayerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Challenge = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChallengeWord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChallengeWord_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LetterStatus",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Letter = table.Column<string>(type: "nvarchar(1)", nullable: false),
                    Color = table.Column<int>(type: "int", nullable: false),
                    ChallengeWordId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LetterStatus", x => x.id);
                    table.ForeignKey(
                        name: "FK_LetterStatus_ChallengeWord_ChallengeWordId",
                        column: x => x.ChallengeWordId,
                        principalTable: "ChallengeWord",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChallengeWord_GameId",
                table: "ChallengeWord",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_LetterStatus_ChallengeWordId",
                table: "LetterStatus",
                column: "ChallengeWordId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LetterStatus");

            migrationBuilder.DropTable(
                name: "ChallengeWord");

            migrationBuilder.DropColumn(
                name: "Guesses",
                table: "Games");

            migrationBuilder.RenameColumn(
                name: "MinLetters",
                table: "Games",
                newName: "Rounds");

            migrationBuilder.RenameColumn(
                name: "MaxLetters",
                table: "Games",
                newName: "CorrectAnswer");
        }
    }
}
