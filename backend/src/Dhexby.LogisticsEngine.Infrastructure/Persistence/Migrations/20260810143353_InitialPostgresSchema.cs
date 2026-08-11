using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dhexby.LogisticsEngine.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgresSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "logistics");

            migrationBuilder.CreateTable(
                name: "hubs",
                schema: "logistics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    latitude = table.Column<double>(type: "double precision", nullable: false),
                    longitude = table.Column<double>(type: "double precision", nullable: false),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hubs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                schema: "logistics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    customer_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    priority = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    assigned_hub_id = table.Column<Guid>(type: "uuid", nullable: true),
                    placed_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roads",
                schema: "logistics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    from_hub_id = table.Column<Guid>(type: "uuid", nullable: false),
                    to_hub_id = table.Column<Guid>(type: "uuid", nullable: false),
                    distance_in_kilometers = table.Column<double>(type: "double precision", nullable: false),
                    traffic_factor = table.Column<double>(type: "double precision", nullable: false),
                    created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roads", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "shipments",
                schema: "logistics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tracking_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    origin = table.Column<string>(type: "jsonb", nullable: false),
                    destination = table.Column<string>(type: "jsonb", nullable: false),
                    weight_in_kilograms = table.Column<decimal>(type: "numeric(9,3)", precision: 9, scale: 3, nullable: false),
                    price_amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    price_currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    is_express = table.Column<bool>(type: "boolean", nullable: false),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    delivered_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shipments", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "logistics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    role = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "zones",
                schema: "logistics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    parent_zone_id = table.Column<Guid>(type: "uuid", nullable: true),
                    streets = table.Column<string>(type: "jsonb", nullable: false),
                    created_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_zones", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_hubs_code",
                schema: "logistics",
                table: "hubs",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_orders_order_number",
                schema: "logistics",
                table: "orders",
                column: "order_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_orders_status",
                schema: "logistics",
                table: "orders",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_roads_from_hub_id_to_hub_id",
                schema: "logistics",
                table: "roads",
                columns: new[] { "from_hub_id", "to_hub_id" });

            migrationBuilder.CreateIndex(
                name: "IX_shipments_status",
                schema: "logistics",
                table: "shipments",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_shipments_tracking_number",
                schema: "logistics",
                table: "shipments",
                column: "tracking_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                schema: "logistics",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_zones_code",
                schema: "logistics",
                table: "zones",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_zones_parent_zone_id",
                schema: "logistics",
                table: "zones",
                column: "parent_zone_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "hubs",
                schema: "logistics");

            migrationBuilder.DropTable(
                name: "orders",
                schema: "logistics");

            migrationBuilder.DropTable(
                name: "roads",
                schema: "logistics");

            migrationBuilder.DropTable(
                name: "shipments",
                schema: "logistics");

            migrationBuilder.DropTable(
                name: "users",
                schema: "logistics");

            migrationBuilder.DropTable(
                name: "zones",
                schema: "logistics");
        }
    }
}
