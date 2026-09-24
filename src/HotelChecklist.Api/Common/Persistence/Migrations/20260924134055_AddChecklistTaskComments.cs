using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelChecklist.Api.Common.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddChecklistTaskComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "checklist_task_comments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    checklist_task_execution_id = table.Column<Guid>(type: "uuid", nullable: false),
                    text = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    author_user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_checklist_task_comments", x => x.id);
                    table.ForeignKey(
                        name: "fk_checklist_task_comments_checklist_task_executions_checklist",
                        column: x => x.checklist_task_execution_id,
                        principalTable: "checklist_task_executions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_checklist_task_comments_users_author_user_id",
                        column: x => x.author_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_checklist_task_comments_author_user_id",
                table: "checklist_task_comments",
                column: "author_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_checklist_task_comments_checklist_task_execution_id",
                table: "checklist_task_comments",
                column: "checklist_task_execution_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "checklist_task_comments");
        }
    }
}
