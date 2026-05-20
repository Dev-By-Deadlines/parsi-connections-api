using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Connections.Api.Migrations
{
    /// <inheritdoc />
    public partial class RefactorState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Outcome",
                table: "GameState",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SolvedCategoryIds",
                table: "GameState",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Outcome",
                table: "GameState");

            migrationBuilder.DropColumn(
                name: "SolvedCategoryIds",
                table: "GameState");
        }
    }
}
