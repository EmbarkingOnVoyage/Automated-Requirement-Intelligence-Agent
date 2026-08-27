using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RequirementAnalysisProject.Migrations
{
    /// <inheritdoc />
    public partial class AddMinutesOfMeeting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MinutesOfMeeting",
                table: "AnalysisResults",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinutesOfMeeting",
                table: "AnalysisResults");
        }
    }
}
