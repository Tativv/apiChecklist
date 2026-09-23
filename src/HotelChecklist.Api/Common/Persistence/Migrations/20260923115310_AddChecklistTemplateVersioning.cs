using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelChecklist.Api.Common.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddChecklistTemplateVersioning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "group_id",
                table: "checklist_templates",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "is_snapshot",
                table: "checklist_templates",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            // Cada template existente pasa a ser la única versión viva de su propio grupo —
            // si se dejara el default de 0s de arriba, todos compartirían group_id y violarían
            // el índice único filtrado de abajo apenas hubiera más de un template.
            migrationBuilder.Sql("UPDATE checklist_templates SET group_id = id;");

            migrationBuilder.CreateIndex(
                name: "ix_checklist_templates_group_id",
                table: "checklist_templates",
                column: "group_id",
                unique: true,
                filter: "is_snapshot = false");

            migrationBuilder.CreateIndex(
                name: "ix_checklist_templates_group_id_is_snapshot",
                table: "checklist_templates",
                columns: new[] { "group_id", "is_snapshot" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_checklist_templates_group_id",
                table: "checklist_templates");

            migrationBuilder.DropIndex(
                name: "ix_checklist_templates_group_id_is_snapshot",
                table: "checklist_templates");

            migrationBuilder.DropColumn(
                name: "group_id",
                table: "checklist_templates");

            migrationBuilder.DropColumn(
                name: "is_snapshot",
                table: "checklist_templates");
        }
    }
}
