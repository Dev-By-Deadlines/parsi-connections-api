using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Connections.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddGameStates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_GameState_GameStateId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Words_GameState_GameStateId",
                table: "Words");

            migrationBuilder.DropIndex(
                name: "IX_Words_GameStateId",
                table: "Words");

            migrationBuilder.DropIndex(
                name: "IX_Categories_GameStateId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "GameStateId",
                table: "Words");

            migrationBuilder.DropColumn(
                name: "GameStateId",
                table: "Categories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GameStateId",
                table: "Words",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GameStateId",
                table: "Categories",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Words_GameStateId",
                table: "Words",
                column: "GameStateId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_GameStateId",
                table: "Categories",
                column: "GameStateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_GameState_GameStateId",
                table: "Categories",
                column: "GameStateId",
                principalTable: "GameState",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Words_GameState_GameStateId",
                table: "Words",
                column: "GameStateId",
                principalTable: "GameState",
                principalColumn: "Id");
        }
    }
}
