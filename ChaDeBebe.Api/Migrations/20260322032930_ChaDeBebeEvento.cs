using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ChaDeBebe.Api.Migrations
{
    /// <inheritdoc />
    public partial class ChaDeBebeEvento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChasDeBebe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    DataEvento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AdminId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChasDeBebe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChasDeBebe_Usuarios_AdminId",
                        column: x => x.AdminId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Presentes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ChaDeBebeEventoId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Presentes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Presentes_ChasDeBebe_ChaDeBebeEventoId",
                        column: x => x.ChaDeBebeEventoId,
                        principalTable: "ChasDeBebe",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChasDeBebe_AdminId",
                table: "ChasDeBebe",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Presentes_ChaDeBebeEventoId",
                table: "Presentes",
                column: "ChaDeBebeEventoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Presentes");

            migrationBuilder.DropTable(
                name: "ChasDeBebe");
        }
    }
}
