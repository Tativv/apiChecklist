using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelChecklist.Api.Common.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RedesignChecklistTaskStatusFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // executed_at_utc ya guardaba "cuándo se completó la tarea" (ver el CompleteChecklistTaskHandler
            // anterior a este cambio) — renombra a completed_at para preservar ese dato con su significado
            // correcto. started_at es un campo nuevo, sin dato previo que migrar.
            migrationBuilder.RenameColumn(
                name: "executed_at_utc",
                table: "checklist_task_executions",
                newName: "completed_at");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "started_at",
                table: "checklist_task_executions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "duration_seconds",
                table: "checklist_task_executions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "estimated_duration_minutes",
                table: "checklist_task_executions",
                type: "integer",
                nullable: true);

            // Backfill de status: 'Approved'/'Reviewed' del checklist y 'Skipped' de la tarea ya no
            // existen como valores válidos del enum — se remapean a los estados nuevos más cercanos.
            migrationBuilder.Sql("UPDATE checklist_instances SET status = 'InProgress' WHERE status = 'Approved';");
            migrationBuilder.Sql("UPDATE checklist_instances SET status = 'Completed' WHERE status = 'Reviewed';");
            migrationBuilder.Sql("UPDATE checklist_task_executions SET status = 'Pending' WHERE status = 'Skipped';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "started_at",
                table: "checklist_task_executions");

            migrationBuilder.DropColumn(
                name: "duration_seconds",
                table: "checklist_task_executions");

            migrationBuilder.DropColumn(
                name: "estimated_duration_minutes",
                table: "checklist_task_executions");

            migrationBuilder.RenameColumn(
                name: "completed_at",
                table: "checklist_task_executions",
                newName: "executed_at_utc");
        }
    }
}
