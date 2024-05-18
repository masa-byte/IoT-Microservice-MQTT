using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EventInfo.Migrations
{
    /// <inheritdoc />
    public partial class V1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PondData",
                columns: table => new
                {
                    EntryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PondId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Temperature_C = table.Column<double>(type: "double precision", nullable: false),
                    pH = table.Column<double>(type: "double precision", nullable: false),
                    DissolvedOxygen_g_mL = table.Column<double>(type: "double precision", nullable: false),
                    Turbidity_ntu = table.Column<double>(type: "double precision", nullable: false),
                    Ammonia_g_mL = table.Column<double>(type: "double precision", nullable: false),
                    Nitrite_g_mL = table.Column<double>(type: "double precision", nullable: false),
                    Population = table.Column<double>(type: "double precision", nullable: false),
                    FishLength_cm = table.Column<double>(type: "double precision", nullable: false),
                    FishWeight_g = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PondData", x => x.EntryId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PondData");
        }
    }
}
