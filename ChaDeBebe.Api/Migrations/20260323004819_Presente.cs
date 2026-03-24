using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ChaDeBebe.Api.Migrations
{
    /// <inheritdoc />
    public partial class Presente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Presentes_ChasDeBebe_ChaDeBebeEventoId",
                table: "Presentes");

            migrationBuilder.AlterColumn<int>(
                name: "ChaDeBebeEventoId",
                table: "Presentes",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Descricao",
                table: "Presentes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkSugerido",
                table: "Presentes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nome",
                table: "Presentes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PathImage",
                table: "Presentes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "QuantidadeTotal",
                table: "Presentes",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "Reserva",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Quantidade = table.Column<int>(type: "integer", nullable: false),
                    DataReserva = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    ChaDeBebeEventoId = table.Column<int>(type: "integer", nullable: false),
                    PresenteId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioChaDeBebeChaDeBebeId = table.Column<int>(type: "integer", nullable: true),
                    UsuarioChaDeBebeUsuarioId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reserva", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reserva_ChasDeBebe_ChaDeBebeEventoId",
                        column: x => x.ChaDeBebeEventoId,
                        principalTable: "ChasDeBebe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reserva_Presentes_PresenteId",
                        column: x => x.PresenteId,
                        principalTable: "Presentes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reserva_UsuarioChaDeBebe_UsuarioChaDeBebeUsuarioId_UsuarioC~",
                        columns: x => new { x.UsuarioChaDeBebeUsuarioId, x.UsuarioChaDeBebeChaDeBebeId },
                        principalTable: "UsuarioChaDeBebe",
                        principalColumns: new[] { "UsuarioId", "ChaDeBebeId" });
                    table.ForeignKey(
                        name: "FK_Reserva_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reserva_ChaDeBebeEventoId",
                table: "Reserva",
                column: "ChaDeBebeEventoId");

            migrationBuilder.CreateIndex(
                name: "IX_Reserva_PresenteId",
                table: "Reserva",
                column: "PresenteId");

            migrationBuilder.CreateIndex(
                name: "IX_Reserva_UsuarioChaDeBebeUsuarioId_UsuarioChaDeBebeChaDeBebe~",
                table: "Reserva",
                columns: new[] { "UsuarioChaDeBebeUsuarioId", "UsuarioChaDeBebeChaDeBebeId" });

            migrationBuilder.CreateIndex(
                name: "IX_Reserva_UsuarioId",
                table: "Reserva",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Presentes_ChasDeBebe_ChaDeBebeEventoId",
                table: "Presentes",
                column: "ChaDeBebeEventoId",
                principalTable: "ChasDeBebe",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Presentes_ChasDeBebe_ChaDeBebeEventoId",
                table: "Presentes");

            migrationBuilder.DropTable(
                name: "Reserva");

            migrationBuilder.DropColumn(
                name: "Descricao",
                table: "Presentes");

            migrationBuilder.DropColumn(
                name: "LinkSugerido",
                table: "Presentes");

            migrationBuilder.DropColumn(
                name: "Nome",
                table: "Presentes");

            migrationBuilder.DropColumn(
                name: "PathImage",
                table: "Presentes");

            migrationBuilder.DropColumn(
                name: "QuantidadeTotal",
                table: "Presentes");

            migrationBuilder.AlterColumn<int>(
                name: "ChaDeBebeEventoId",
                table: "Presentes",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Presentes_ChasDeBebe_ChaDeBebeEventoId",
                table: "Presentes",
                column: "ChaDeBebeEventoId",
                principalTable: "ChasDeBebe",
                principalColumn: "Id");
        }
    }
}
