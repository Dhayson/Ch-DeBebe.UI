using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChaDeBebe.Api.Migrations
{
    /// <inheritdoc />
    public partial class ChaDeBebeId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioChaDeBebe_ChasDeBebe_ChaDeBebeEventoId",
                table: "UsuarioChaDeBebe");

            migrationBuilder.DropIndex(
                name: "IX_UsuarioChaDeBebe_ChaDeBebeEventoId",
                table: "UsuarioChaDeBebe");

            migrationBuilder.DropColumn(
                name: "ChaDeBebeEventoId",
                table: "UsuarioChaDeBebe");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioChaDeBebe_ChaDeBebeId",
                table: "UsuarioChaDeBebe",
                column: "ChaDeBebeId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioChaDeBebe_ChasDeBebe_ChaDeBebeId",
                table: "UsuarioChaDeBebe",
                column: "ChaDeBebeId",
                principalTable: "ChasDeBebe",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioChaDeBebe_ChasDeBebe_ChaDeBebeId",
                table: "UsuarioChaDeBebe");

            migrationBuilder.DropIndex(
                name: "IX_UsuarioChaDeBebe_ChaDeBebeId",
                table: "UsuarioChaDeBebe");

            migrationBuilder.AddColumn<int>(
                name: "ChaDeBebeEventoId",
                table: "UsuarioChaDeBebe",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioChaDeBebe_ChaDeBebeEventoId",
                table: "UsuarioChaDeBebe",
                column: "ChaDeBebeEventoId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioChaDeBebe_ChasDeBebe_ChaDeBebeEventoId",
                table: "UsuarioChaDeBebe",
                column: "ChaDeBebeEventoId",
                principalTable: "ChasDeBebe",
                principalColumn: "Id");
        }
    }
}
