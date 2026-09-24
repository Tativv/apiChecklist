using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelChecklist.Api.Common.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DropChecklistTemplateEstimatedDuration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "estimated_duration_minutes",
                table: "checklist_templates");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "estimated_duration_minutes",
                table: "checklist_templates",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
