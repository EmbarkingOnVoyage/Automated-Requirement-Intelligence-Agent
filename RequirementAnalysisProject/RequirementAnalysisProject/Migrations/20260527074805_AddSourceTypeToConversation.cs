using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RequirementAnalysisProject.Migrations
{
    /// <inheritdoc />
    public partial class AddSourceTypeToConversation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SourceType",
                table: "Conversations",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SourceType",
                table: "Conversations");
        }
    }
}
