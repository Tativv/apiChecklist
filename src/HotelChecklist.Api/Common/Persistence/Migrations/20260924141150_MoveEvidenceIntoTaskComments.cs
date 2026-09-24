using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelChecklist.Api.Common.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MoveEvidenceIntoTaskComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "text",
                table: "checklist_task_comments",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AddColumn<string>(
                name: "content_type",
                table: "checklist_task_comments",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "file_name",
                table: "checklist_task_comments",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "file_path",
                table: "checklist_task_comments",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "file_size_bytes",
                table: "checklist_task_comments",
                type: "bigint",
                nullable: true);

            // Cada evidencia existente se preserva como un comentario sin texto, con el mismo id,
            // autor (uploaded_by_user_id -> author_user_id) y fecha (uploaded_at -> created_at) —
            // la evidencia deja de ser una entidad propia y pasa a ser "un comentario con archivo".
            migrationBuilder.Sql(
                """
                INSERT INTO checklist_task_comments (id, checklist_task_execution_id, text, created_at, author_user_id, file_path, file_name, content_type, file_size_bytes)
                SELECT id, checklist_task_execution_id, NULL, uploaded_at, uploaded_by_user_id, file_path, file_name, content_type, file_size_bytes
                FROM checklist_task_evidences;
                """);

            migrationBuilder.DropTable(
                name: "checklist_task_evidences");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "content_type",
                table: "checklist_task_comments");

            migrationBuilder.DropColumn(
                name: "file_name",
                table: "checklist_task_comments");

            migrationBuilder.DropColumn(
                name: "file_path",
                table: "checklist_task_comments");

            migrationBuilder.DropColumn(
                name: "file_size_bytes",
                table: "checklist_task_comments");

            migrationBuilder.AlterColumn<string>(
                name: "text",
                table: "checklist_task_comments",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "checklist_task_evidences",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    checklist_task_execution_id = table.Column<Guid>(type: "uuid", nullable: false),
                    uploaded_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    content_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    file_name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    file_path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    file_size_bytes = table.Column<long>(type: "bigint", nullable: false),
                    uploaded_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_checklist_task_evidences", x => x.id);
                    table.ForeignKey(
                        name: "fk_checklist_task_evidences_checklist_task_executions_checklis",
                        column: x => x.checklist_task_execution_id,
                        principalTable: "checklist_task_executions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_checklist_task_evidences_users_uploaded_by_user_id",
                        column: x => x.uploaded_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_checklist_task_evidences_checklist_task_execution_id",
                table: "checklist_task_evidences",
                column: "checklist_task_execution_id");

            migrationBuilder.CreateIndex(
                name: "ix_checklist_task_evidences_uploaded_by_user_id",
                table: "checklist_task_evidences",
                column: "uploaded_by_user_id");
        }
    }
}
