using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelChecklist.Api.Common.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConvertToGenericSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_checklist_instances_template_id",
                table: "checklist_instances");

            // --- 1) Add new columns/tables. Old recurrence columns are kept for now: the data
            // conversion below still needs to read them. They are dropped at the very end. ---

            migrationBuilder.AddColumn<string>(
                name: "execution_mode",
                table: "checklist_tasks",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Scheduled");

            migrationBuilder.AddColumn<Guid>(
                name: "completed_by_user_id",
                table: "checklist_task_executions",
                type: "uuid",
                nullable: true);

            migrationBuilder.RenameColumn(
                name: "completed_at",
                table: "checklist_task_executions",
                newName: "executed_at_utc");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "scheduled_for_utc",
                table: "checklist_task_executions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "schedule_id",
                table: "checklist_task_executions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "checklist_task_executions",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Pending");

            migrationBuilder.CreateTable(
                name: "schedules",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    frequency_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    interval_value = table.Column<int>(type: "integer", nullable: false),
                    week_day = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    day_of_month = table.Column<int>(type: "integer", nullable: true),
                    time_of_day = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    execution_order = table.Column<int>(type: "integer", nullable: false),
                    active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_schedules", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "task_schedules",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    task_id = table.Column<Guid>(type: "uuid", nullable: false),
                    schedule_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_task_schedules", x => x.id);
                    table.ForeignKey(
                        name: "fk_task_schedules_checklist_tasks_task_id",
                        column: x => x.task_id,
                        principalTable: "checklist_tasks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_task_schedules_schedules_schedule_id",
                        column: x => x.schedule_id,
                        principalTable: "schedules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "template_schedules",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    template_id = table.Column<Guid>(type: "uuid", nullable: false),
                    schedule_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_template_schedules", x => x.id);
                    table.ForeignKey(
                        name: "fk_template_schedules_checklist_templates_template_id",
                        column: x => x.template_id,
                        principalTable: "checklist_templates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_template_schedules_schedules_schedule_id",
                        column: x => x.schedule_id,
                        principalTable: "schedules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            // --- 2) Convert existing data: one Schedule (+ TemplateSchedule) per existing
            // recurrence rule, and one Schedule (+ TaskSchedule) per existing task, cloning the
            // template's own time of day. This preserves current behavior exactly, since the
            // instance generator only reads TaskSchedule.Schedule.TimeOfDay, never its frequency. ---

            migrationBuilder.Sql(@"
                DO $$
                DECLARE
                    tmpl RECORD;
                    task_rec RECORD;
                    v_schedule_id uuid;
                    day_bit int;
                    day_names text[] := ARRAY['Sunday','Monday','Tuesday','Wednesday','Thursday','Friday','Saturday'];
                BEGIN
                    FOR tmpl IN
                        SELECT id, recurrence_type, custom_recurrence_mode, recurrence_interval_value,
                               recurrence_interval_unit, recurrence_days_of_week_mask, scheduled_time, recurrence_start_date
                        FROM checklist_templates
                    LOOP
                        IF tmpl.recurrence_type = 0 THEN -- Daily
                            v_schedule_id := gen_random_uuid();
                            INSERT INTO schedules (id, frequency_type, interval_value, week_day, day_of_month, time_of_day, execution_order, active, created_at_utc)
                            VALUES (v_schedule_id, 'Daily', 1, NULL, NULL, tmpl.scheduled_time, 0, true, tmpl.recurrence_start_date::timestamptz);
                            INSERT INTO template_schedules (id, template_id, schedule_id) VALUES (gen_random_uuid(), tmpl.id, v_schedule_id);

                        ELSIF tmpl.recurrence_type = 1 THEN -- Weekly
                            v_schedule_id := gen_random_uuid();
                            INSERT INTO schedules (id, frequency_type, interval_value, week_day, day_of_month, time_of_day, execution_order, active, created_at_utc)
                            VALUES (v_schedule_id, 'Weekly', 1, day_names[EXTRACT(DOW FROM tmpl.recurrence_start_date)::int + 1], NULL, tmpl.scheduled_time, 0, true, tmpl.recurrence_start_date::timestamptz);
                            INSERT INTO template_schedules (id, template_id, schedule_id) VALUES (gen_random_uuid(), tmpl.id, v_schedule_id);

                        ELSIF tmpl.recurrence_type = 2 THEN -- Monthly
                            v_schedule_id := gen_random_uuid();
                            INSERT INTO schedules (id, frequency_type, interval_value, week_day, day_of_month, time_of_day, execution_order, active, created_at_utc)
                            VALUES (v_schedule_id, 'Monthly', 1, NULL, EXTRACT(DAY FROM tmpl.recurrence_start_date)::int, tmpl.scheduled_time, 0, true, tmpl.recurrence_start_date::timestamptz);
                            INSERT INTO template_schedules (id, template_id, schedule_id) VALUES (gen_random_uuid(), tmpl.id, v_schedule_id);

                        ELSIF tmpl.recurrence_type = 3 THEN -- Custom
                            IF tmpl.custom_recurrence_mode = 0 THEN -- Interval
                                v_schedule_id := gen_random_uuid();
                                INSERT INTO schedules (id, frequency_type, interval_value, week_day, day_of_month, time_of_day, execution_order, active, created_at_utc)
                                VALUES (
                                    v_schedule_id,
                                    CASE tmpl.recurrence_interval_unit WHEN 0 THEN 'Daily' WHEN 1 THEN 'Weekly' WHEN 2 THEN 'Monthly' ELSE 'Daily' END,
                                    COALESCE(tmpl.recurrence_interval_value, 1),
                                    CASE WHEN tmpl.recurrence_interval_unit = 1 THEN day_names[EXTRACT(DOW FROM tmpl.recurrence_start_date)::int + 1] ELSE NULL END,
                                    CASE WHEN tmpl.recurrence_interval_unit = 2 THEN EXTRACT(DAY FROM tmpl.recurrence_start_date)::int ELSE NULL END,
                                    tmpl.scheduled_time, 0, true, tmpl.recurrence_start_date::timestamptz);
                                INSERT INTO template_schedules (id, template_id, schedule_id) VALUES (gen_random_uuid(), tmpl.id, v_schedule_id);

                            ELSIF tmpl.custom_recurrence_mode = 1 THEN -- DaysOfWeek
                                FOR day_bit IN 0..6 LOOP
                                    IF (tmpl.recurrence_days_of_week_mask & (1 << day_bit)) != 0 THEN
                                        v_schedule_id := gen_random_uuid();
                                        INSERT INTO schedules (id, frequency_type, interval_value, week_day, day_of_month, time_of_day, execution_order, active, created_at_utc)
                                        VALUES (v_schedule_id, 'Weekly', 1, day_names[day_bit + 1], NULL, tmpl.scheduled_time, day_bit, true, tmpl.recurrence_start_date::timestamptz);
                                        INSERT INTO template_schedules (id, template_id, schedule_id) VALUES (gen_random_uuid(), tmpl.id, v_schedule_id);
                                    END IF;
                                END LOOP;
                            END IF;
                        END IF;

                        -- Every existing task becomes Scheduled with a single TaskSchedule cloning
                        -- the template's own time of day (the generator only reads TimeOfDay).
                        FOR task_rec IN SELECT id FROM checklist_tasks WHERE template_id = tmpl.id LOOP
                            v_schedule_id := gen_random_uuid();
                            INSERT INTO schedules (id, frequency_type, interval_value, week_day, day_of_month, time_of_day, execution_order, active, created_at_utc)
                            VALUES (v_schedule_id, 'Daily', 1, NULL, NULL, tmpl.scheduled_time, 0, true, tmpl.recurrence_start_date::timestamptz);
                            INSERT INTO task_schedules (id, task_id, schedule_id) VALUES (gen_random_uuid(), task_rec.id, v_schedule_id);
                        END LOOP;
                    END LOOP;
                END $$;

                UPDATE checklist_task_executions SET status = CASE WHEN completed THEN 'Completed' ELSE 'Pending' END;
            ");

            // --- 3) Drop the old columns now that their data has been converted. ---

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
                name: "recurrence_type",
                table: "checklist_templates");

            migrationBuilder.DropColumn(
                name: "scheduled_time",
                table: "checklist_templates");

            migrationBuilder.DropColumn(
                name: "completed",
                table: "checklist_task_executions");

            // --- 4) Indexes and foreign keys. ---

            migrationBuilder.CreateIndex(
                name: "ix_checklist_task_executions_checklist_instance_id_task_id",
                table: "checklist_task_executions",
                columns: new[] { "checklist_instance_id", "task_id" });

            migrationBuilder.CreateIndex(
                name: "ix_checklist_task_executions_completed_by_user_id",
                table: "checklist_task_executions",
                column: "completed_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_checklist_task_executions_schedule_id",
                table: "checklist_task_executions",
                column: "schedule_id");

            migrationBuilder.CreateIndex(
                name: "ix_checklist_instances_template_id_asset_id_date",
                table: "checklist_instances",
                columns: new[] { "template_id", "asset_id", "date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_schedules_frequency_type_active",
                table: "schedules",
                columns: new[] { "frequency_type", "active" });

            migrationBuilder.CreateIndex(
                name: "ix_task_schedules_schedule_id",
                table: "task_schedules",
                column: "schedule_id");

            migrationBuilder.CreateIndex(
                name: "ix_task_schedules_task_id",
                table: "task_schedules",
                column: "task_id");

            migrationBuilder.CreateIndex(
                name: "ix_template_schedules_schedule_id",
                table: "template_schedules",
                column: "schedule_id");

            migrationBuilder.CreateIndex(
                name: "ix_template_schedules_template_id",
                table: "template_schedules",
                column: "template_id");

            migrationBuilder.AddForeignKey(
                name: "fk_checklist_task_executions_schedules_schedule_id",
                table: "checklist_task_executions",
                column: "schedule_id",
                principalTable: "schedules",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_checklist_task_executions_users_completed_by_user_id",
                table: "checklist_task_executions",
                column: "completed_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_checklist_task_executions_schedules_schedule_id",
                table: "checklist_task_executions");

            migrationBuilder.DropForeignKey(
                name: "fk_checklist_task_executions_users_completed_by_user_id",
                table: "checklist_task_executions");

            migrationBuilder.DropTable(
                name: "task_schedules");

            migrationBuilder.DropTable(
                name: "template_schedules");

            migrationBuilder.DropTable(
                name: "schedules");

            migrationBuilder.DropIndex(
                name: "ix_checklist_task_executions_checklist_instance_id_task_id",
                table: "checklist_task_executions");

            migrationBuilder.DropIndex(
                name: "ix_checklist_task_executions_completed_by_user_id",
                table: "checklist_task_executions");

            migrationBuilder.DropIndex(
                name: "ix_checklist_task_executions_schedule_id",
                table: "checklist_task_executions");

            migrationBuilder.DropIndex(
                name: "ix_checklist_instances_template_id_asset_id_date",
                table: "checklist_instances");

            migrationBuilder.DropColumn(
                name: "execution_mode",
                table: "checklist_tasks");

            migrationBuilder.DropColumn(
                name: "completed_by_user_id",
                table: "checklist_task_executions");

            migrationBuilder.DropColumn(
                name: "scheduled_for_utc",
                table: "checklist_task_executions");

            migrationBuilder.DropColumn(
                name: "schedule_id",
                table: "checklist_task_executions");

            migrationBuilder.DropColumn(
                name: "status",
                table: "checklist_task_executions");

            migrationBuilder.RenameColumn(
                name: "executed_at_utc",
                table: "checklist_task_executions",
                newName: "completed_at");

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

            migrationBuilder.AddColumn<int>(
                name: "recurrence_type",
                table: "checklist_templates",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "scheduled_time",
                table: "checklist_templates",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<bool>(
                name: "completed",
                table: "checklist_task_executions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "ix_checklist_instances_template_id",
                table: "checklist_instances",
                column: "template_id");
        }
    }
}
