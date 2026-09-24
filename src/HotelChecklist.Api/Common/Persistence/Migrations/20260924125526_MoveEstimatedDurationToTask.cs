using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelChecklist.Api.Common.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MoveEstimatedDurationToTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "estimated_duration_minutes",
                table: "checklist_task_executions");

            migrationBuilder.AddColumn<int>(
                name: "estimated_duration_minutes",
                table: "checklist_tasks",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "estimated_duration_minutes",
                table: "checklist_tasks");

            migrationBuilder.AddColumn<int>(
                name: "estimated_duration_minutes",
                table: "checklist_task_executions",
                type: "integer",
                nullable: true);
        }
    }
}
