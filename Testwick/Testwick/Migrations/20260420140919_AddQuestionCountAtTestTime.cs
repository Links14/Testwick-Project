using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Testwick.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestionCountAtTestTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuestionCountAtTestTime",
                table: "QuizResults",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuestionCountAtTestTime",
                table: "QuizResults");
        }
    }
}
