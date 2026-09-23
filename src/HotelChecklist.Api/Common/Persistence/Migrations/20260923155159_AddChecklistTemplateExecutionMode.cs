using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelChecklist.Api.Common.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddChecklistTemplateExecutionMode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "execution_mode",
                table: "checklist_templates",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Scheduled");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "execution_mode",
                table: "checklist_templates");
        }
    }
}
