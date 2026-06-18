using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Connections.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddEmojiGridToState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GuessGrid",
                table: "GameStates",
                type: "TEXT",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GuessGrid",
                table: "GameStates");
        }
    }
}
