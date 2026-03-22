using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChaDeBebe.Api.Migrations
{
    /// <inheritdoc />
    public partial class UsuarioCha : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Usuarios",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Usuarios",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "UsuarioChaDeBebe",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    ChaDeBebeId = table.Column<int>(type: "integer", nullable: false),
                    ChaDeBebeEventoId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioChaDeBebe", x => new { x.UsuarioId, x.ChaDeBebeId });
                    table.ForeignKey(
                        name: "FK_UsuarioChaDeBebe_ChasDeBebe_ChaDeBebeEventoId",
                        column: x => x.ChaDeBebeEventoId,
                        principalTable: "ChasDeBebe",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UsuarioChaDeBebe_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioChaDeBebe_ChaDeBebeEventoId",
                table: "UsuarioChaDeBebe",
                column: "ChaDeBebeEventoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsuarioChaDeBebe");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Usuarios",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Usuarios",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);
        }
    }
}
