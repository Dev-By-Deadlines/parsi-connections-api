using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Connections.Api.Migrations
{
    /// <inheritdoc />
    public partial class FixSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Puzzles_PuzzleId",
                table: "Categories");

            migrationBuilder.AlterColumn<int>(
                name: "PuzzleId",
                table: "Categories",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Puzzles_PuzzleId",
                table: "Categories",
                column: "PuzzleId",
                principalTable: "Puzzles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Puzzles_PuzzleId",
                table: "Categories");

            migrationBuilder.AlterColumn<int>(
                name: "PuzzleId",
                table: "Categories",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Puzzles_PuzzleId",
                table: "Categories",
                column: "PuzzleId",
                principalTable: "Puzzles",
                principalColumn: "Id");
        }
    }
}
