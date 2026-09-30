using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelChecklist.Api.Common.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "service_orders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    area_id = table.Column<Guid>(type: "uuid", nullable: false),
                    asset_id = table.Column<Guid>(type: "uuid", nullable: false),
                    call_id = table.Column<Guid>(type: "uuid", nullable: true),
                    subject = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    priority = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    due_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    assigned_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    started_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    duration_seconds = table.Column<long>(type: "bigint", nullable: true),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_service_orders", x => x.id);
                    table.ForeignKey(
                        name: "fk_service_orders_areas_area_id",
                        column: x => x.area_id,
                        principalTable: "areas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_service_orders_assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "assets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_service_orders_calls_call_id",
                        column: x => x.call_id,
                        principalTable: "calls",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_service_orders_users_assigned_user_id",
                        column: x => x.assigned_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_service_orders_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "service_order_comments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    text = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    author_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    file_name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    content_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    file_size_bytes = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_service_order_comments", x => x.id);
                    table.ForeignKey(
                        name: "fk_service_order_comments_service_orders_service_order_id",
                        column: x => x.service_order_id,
                        principalTable: "service_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_service_order_comments_users_author_user_id",
                        column: x => x.author_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_service_order_comments_author_user_id",
                table: "service_order_comments",
                column: "author_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_service_order_comments_service_order_id",
                table: "service_order_comments",
                column: "service_order_id");

            migrationBuilder.CreateIndex(
                name: "ix_service_orders_area_id",
                table: "service_orders",
                column: "area_id");

            migrationBuilder.CreateIndex(
                name: "ix_service_orders_asset_id",
                table: "service_orders",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "ix_service_orders_assigned_user_id",
                table: "service_orders",
                column: "assigned_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_service_orders_call_id",
                table: "service_orders",
                column: "call_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_service_orders_created_by_user_id",
                table: "service_orders",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_service_orders_status",
                table: "service_orders",
                column: "status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "service_order_comments");

            migrationBuilder.DropTable(
                name: "service_orders");
        }
    }
}
