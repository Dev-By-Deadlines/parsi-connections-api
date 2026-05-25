using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Connections.Api.Migrations
{
    /// <inheritdoc />
    public partial class RenameSomeTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyPuzzle_Puzzles_PuzzleId",
                table: "DailyPuzzle");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GameState",
                table: "GameState");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DailyPuzzle",
                table: "DailyPuzzle");

            migrationBuilder.RenameTable(
                name: "GameState",
                newName: "GameStates");

            migrationBuilder.RenameTable(
                name: "DailyPuzzle",
                newName: "DailyPuzzles");

            migrationBuilder.RenameIndex(
                name: "IX_DailyPuzzle_PuzzleId",
                table: "DailyPuzzles",
                newName: "IX_DailyPuzzles_PuzzleId");

            migrationBuilder.RenameIndex(
                name: "IX_DailyPuzzle_Date",
                table: "DailyPuzzles",
                newName: "IX_DailyPuzzles_Date");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GameStates",
                table: "GameStates",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DailyPuzzles",
                table: "DailyPuzzles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DailyPuzzles_Puzzles_PuzzleId",
                table: "DailyPuzzles",
                column: "PuzzleId",
                principalTable: "Puzzles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyPuzzles_Puzzles_PuzzleId",
                table: "DailyPuzzles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GameStates",
                table: "GameStates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DailyPuzzles",
                table: "DailyPuzzles");

            migrationBuilder.RenameTable(
                name: "GameStates",
                newName: "GameState");

            migrationBuilder.RenameTable(
                name: "DailyPuzzles",
                newName: "DailyPuzzle");

            migrationBuilder.RenameIndex(
                name: "IX_DailyPuzzles_PuzzleId",
                table: "DailyPuzzle",
                newName: "IX_DailyPuzzle_PuzzleId");

            migrationBuilder.RenameIndex(
                name: "IX_DailyPuzzles_Date",
                table: "DailyPuzzle",
                newName: "IX_DailyPuzzle_Date");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GameState",
                table: "GameState",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DailyPuzzle",
                table: "DailyPuzzle",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DailyPuzzle_Puzzles_PuzzleId",
                table: "DailyPuzzle",
                column: "PuzzleId",
                principalTable: "Puzzles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
