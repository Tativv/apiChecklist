using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelChecklist.Api.Common.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AssignTasksToUsersAndUserAreas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE users
                SET role = CASE role
                    WHEN 'Admin' THEN 'Directoria'
                    WHEN 'Manager' THEN 'Gerencia'
                    WHEN 'Operator' THEN 'Colaborador'
                    ELSE role
                END;
            ");

            migrationBuilder.DropForeignKey(
                name: "fk_checklist_instances_users_approved_by_user_id",
                table: "checklist_instances");

            migrationBuilder.DropForeignKey(
                name: "fk_checklist_instances_users_assigned_user_id",
                table: "checklist_instances");

            migrationBuilder.DropForeignKey(
                name: "fk_checklist_task_executions_users_completed_by_user_id",
                table: "checklist_task_executions");

            migrationBuilder.DropIndex(
                name: "ix_checklist_instances_approved_by_user_id",
                table: "checklist_instances");

            migrationBuilder.DropIndex(
                name: "ix_checklist_instances_assigned_user_id",
                table: "checklist_instances");

            migrationBuilder.DropColumn(
                name: "approved_at",
                table: "checklist_instances");

            migrationBuilder.DropColumn(
                name: "approved_by_user_id",
                table: "checklist_instances");

            migrationBuilder.DropColumn(
                name: "assigned_user_id",
                table: "checklist_instances");

            migrationBuilder.RenameColumn(
                name: "completed_by_user_id",
                table: "checklist_task_executions",
                newName: "executed_by_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_checklist_task_executions_completed_by_user_id",
                table: "checklist_task_executions",
                newName: "ix_checklist_task_executions_executed_by_user_id");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "approved_at",
                table: "checklist_task_executions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "approved_by_user_id",
                table: "checklist_task_executions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "assigned_user_id",
                table: "checklist_task_executions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "created_by_user_id",
                table: "checklist_task_executions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "user_areas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    area_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_areas", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_areas_areas_area_id",
                        column: x => x.area_id,
                        principalTable: "areas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_areas_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_checklist_task_executions_approved_by_user_id",
                table: "checklist_task_executions",
                column: "approved_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_checklist_task_executions_assigned_user_id",
                table: "checklist_task_executions",
                column: "assigned_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_checklist_task_executions_created_by_user_id",
                table: "checklist_task_executions",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_areas_area_id",
                table: "user_areas",
                column: "area_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_areas_user_id_area_id",
                table: "user_areas",
                columns: new[] { "user_id", "area_id" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_checklist_task_executions_users_approved_by_user_id",
                table: "checklist_task_executions",
                column: "approved_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_checklist_task_executions_users_assigned_user_id",
                table: "checklist_task_executions",
                column: "assigned_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_checklist_task_executions_users_created_by_user_id",
                table: "checklist_task_executions",
                column: "created_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_checklist_task_executions_users_executed_by_user_id",
                table: "checklist_task_executions",
                column: "executed_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_checklist_task_executions_users_approved_by_user_id",
                table: "checklist_task_executions");

            migrationBuilder.DropForeignKey(
                name: "fk_checklist_task_executions_users_assigned_user_id",
                table: "checklist_task_executions");

            migrationBuilder.DropForeignKey(
                name: "fk_checklist_task_executions_users_created_by_user_id",
                table: "checklist_task_executions");

            migrationBuilder.DropForeignKey(
                name: "fk_checklist_task_executions_users_executed_by_user_id",
                table: "checklist_task_executions");

            migrationBuilder.DropTable(
                name: "user_areas");

            migrationBuilder.DropIndex(
                name: "ix_checklist_task_executions_approved_by_user_id",
                table: "checklist_task_executions");

            migrationBuilder.DropIndex(
                name: "ix_checklist_task_executions_assigned_user_id",
                table: "checklist_task_executions");

            migrationBuilder.DropIndex(
                name: "ix_checklist_task_executions_created_by_user_id",
                table: "checklist_task_executions");

            migrationBuilder.DropColumn(
                name: "approved_at",
                table: "checklist_task_executions");

            migrationBuilder.DropColumn(
                name: "approved_by_user_id",
                table: "checklist_task_executions");

            migrationBuilder.DropColumn(
                name: "assigned_user_id",
                table: "checklist_task_executions");

            migrationBuilder.DropColumn(
                name: "created_by_user_id",
                table: "checklist_task_executions");

            migrationBuilder.RenameColumn(
                name: "executed_by_user_id",
                table: "checklist_task_executions",
                newName: "completed_by_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_checklist_task_executions_executed_by_user_id",
                table: "checklist_task_executions",
                newName: "ix_checklist_task_executions_completed_by_user_id");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "approved_at",
                table: "checklist_instances",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "approved_by_user_id",
                table: "checklist_instances",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "assigned_user_id",
                table: "checklist_instances",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_checklist_instances_approved_by_user_id",
                table: "checklist_instances",
                column: "approved_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_checklist_instances_assigned_user_id",
                table: "checklist_instances",
                column: "assigned_user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_checklist_instances_users_approved_by_user_id",
                table: "checklist_instances",
                column: "approved_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_checklist_instances_users_assigned_user_id",
                table: "checklist_instances",
                column: "assigned_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_checklist_task_executions_users_completed_by_user_id",
                table: "checklist_task_executions",
                column: "completed_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql(@"
                UPDATE users
                SET role = CASE role
                    WHEN 'Directoria' THEN 'Admin'
                    WHEN 'Gerencia' THEN 'Manager'
                    WHEN 'Colaborador' THEN 'Operator'
                    ELSE role
                END;
            ");
        }
    }
}
