using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelChecklist.Api.Common.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTemplateSchedulingAndAssets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "custom_recurrence_mode",
                table: "checklist_templates",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "recurrence_days_of_week_mask",
                table: "checklist_templates",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "recurrence_interval_unit",
                table: "checklist_templates",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "recurrence_interval_value",
                table: "checklist_templates",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "recurrence_start_date",
                table: "checklist_templates",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "scheduled_time",
                table: "checklist_templates",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "checklist_tasks",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "template_assets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    template_id = table.Column<Guid>(type: "uuid", nullable: false),
                    asset_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_template_assets", x => x.id);
                    table.ForeignKey(
                        name: "fk_template_assets_assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "assets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_template_assets_checklist_templates_template_id",
                        column: x => x.template_id,
                        principalTable: "checklist_templates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_template_assets_asset_id",
                table: "template_assets",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "ix_template_assets_template_id_asset_id",
                table: "template_assets",
                columns: new[] { "template_id", "asset_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "template_assets");

            migrationBuilder.DropColumn(
                name: "custom_recurrence_mode",
                table: "checklist_templates");

            migrationBuilder.DropColumn(
                name: "recurrence_days_of_week_mask",
                table: "checklist_templates");

            migrationBuilder.DropColumn(
                name: "recurrence_interval_unit",
                table: "checklist_templates");

            migrationBuilder.DropColumn(
                name: "recurrence_interval_value",
                table: "checklist_templates");

            migrationBuilder.DropColumn(
                name: "recurrence_start_date",
                table: "checklist_templates");

            migrationBuilder.DropColumn(
                name: "scheduled_time",
                table: "checklist_templates");

            migrationBuilder.DropColumn(
                name: "description",
                table: "checklist_tasks");
        }
    }
}
