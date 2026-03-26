using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ChaDeBebe.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    HashSenha = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

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
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Preco = table.Column<decimal>(type: "numeric", nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: true),
                    LinkSugerido = table.Column<string>(type: "text", nullable: true),
                    PathImage = table.Column<string>(type: "text", nullable: true),
                    ChaDeBebeEventoId = table.Column<int>(type: "integer", nullable: false),
                    QuantidadeTotal = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Presentes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Presentes_ChasDeBebe_ChaDeBebeEventoId",
                        column: x => x.ChaDeBebeEventoId,
                        principalTable: "ChasDeBebe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioChaDeBebe",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    ChaDeBebeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioChaDeBebe", x => new { x.UsuarioId, x.ChaDeBebeId });
                    table.ForeignKey(
                        name: "FK_UsuarioChaDeBebe_ChasDeBebe_ChaDeBebeId",
                        column: x => x.ChaDeBebeId,
                        principalTable: "ChasDeBebe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioChaDeBebe_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reservas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Quantidade = table.Column<decimal>(type: "numeric", nullable: false),
                    DataReserva = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    ChaDeBebeEventoId = table.Column<int>(type: "integer", nullable: false),
                    PresenteId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioChaDeBebeChaDeBebeId = table.Column<int>(type: "integer", nullable: true),
                    UsuarioChaDeBebeUsuarioId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservas_ChasDeBebe_ChaDeBebeEventoId",
                        column: x => x.ChaDeBebeEventoId,
                        principalTable: "ChasDeBebe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservas_Presentes_PresenteId",
                        column: x => x.PresenteId,
                        principalTable: "Presentes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservas_UsuarioChaDeBebe_UsuarioChaDeBebeUsuarioId_Usuario~",
                        columns: x => new { x.UsuarioChaDeBebeUsuarioId, x.UsuarioChaDeBebeChaDeBebeId },
                        principalTable: "UsuarioChaDeBebe",
                        principalColumns: new[] { "UsuarioId", "ChaDeBebeId" });
                    table.ForeignKey(
                        name: "FK_Reservas_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChasDeBebe_AdminId",
                table: "ChasDeBebe",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Presentes_ChaDeBebeEventoId",
                table: "Presentes",
                column: "ChaDeBebeEventoId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_ChaDeBebeEventoId",
                table: "Reservas",
                column: "ChaDeBebeEventoId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_PresenteId",
                table: "Reservas",
                column: "PresenteId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_UsuarioChaDeBebeUsuarioId_UsuarioChaDeBebeChaDeBeb~",
                table: "Reservas",
                columns: new[] { "UsuarioChaDeBebeUsuarioId", "UsuarioChaDeBebeChaDeBebeId" });

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_UsuarioId",
                table: "Reservas",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioChaDeBebe_ChaDeBebeId",
                table: "UsuarioChaDeBebe",
                column: "ChaDeBebeId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reservas");

            migrationBuilder.DropTable(
                name: "Presentes");

            migrationBuilder.DropTable(
                name: "UsuarioChaDeBebe");

            migrationBuilder.DropTable(
                name: "ChasDeBebe");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
