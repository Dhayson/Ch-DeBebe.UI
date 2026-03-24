using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChaDeBebe.Api.Migrations
{
    /// <inheritdoc />
    public partial class Reserva : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reserva_ChasDeBebe_ChaDeBebeEventoId",
                table: "Reserva");

            migrationBuilder.DropForeignKey(
                name: "FK_Reserva_Presentes_PresenteId",
                table: "Reserva");

            migrationBuilder.DropForeignKey(
                name: "FK_Reserva_UsuarioChaDeBebe_UsuarioChaDeBebeUsuarioId_UsuarioC~",
                table: "Reserva");

            migrationBuilder.DropForeignKey(
                name: "FK_Reserva_Usuarios_UsuarioId",
                table: "Reserva");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reserva",
                table: "Reserva");

            migrationBuilder.RenameTable(
                name: "Reserva",
                newName: "Reservas");

            migrationBuilder.RenameIndex(
                name: "IX_Reserva_UsuarioId",
                table: "Reservas",
                newName: "IX_Reservas_UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_Reserva_UsuarioChaDeBebeUsuarioId_UsuarioChaDeBebeChaDeBebe~",
                table: "Reservas",
                newName: "IX_Reservas_UsuarioChaDeBebeUsuarioId_UsuarioChaDeBebeChaDeBeb~");

            migrationBuilder.RenameIndex(
                name: "IX_Reserva_PresenteId",
                table: "Reservas",
                newName: "IX_Reservas_PresenteId");

            migrationBuilder.RenameIndex(
                name: "IX_Reserva_ChaDeBebeEventoId",
                table: "Reservas",
                newName: "IX_Reservas_ChaDeBebeEventoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reservas",
                table: "Reservas",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_ChasDeBebe_ChaDeBebeEventoId",
                table: "Reservas",
                column: "ChaDeBebeEventoId",
                principalTable: "ChasDeBebe",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Presentes_PresenteId",
                table: "Reservas",
                column: "PresenteId",
                principalTable: "Presentes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_UsuarioChaDeBebe_UsuarioChaDeBebeUsuarioId_Usuario~",
                table: "Reservas",
                columns: new[] { "UsuarioChaDeBebeUsuarioId", "UsuarioChaDeBebeChaDeBebeId" },
                principalTable: "UsuarioChaDeBebe",
                principalColumns: new[] { "UsuarioId", "ChaDeBebeId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Usuarios_UsuarioId",
                table: "Reservas",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_ChasDeBebe_ChaDeBebeEventoId",
                table: "Reservas");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Presentes_PresenteId",
                table: "Reservas");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_UsuarioChaDeBebe_UsuarioChaDeBebeUsuarioId_Usuario~",
                table: "Reservas");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Usuarios_UsuarioId",
                table: "Reservas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reservas",
                table: "Reservas");

            migrationBuilder.RenameTable(
                name: "Reservas",
                newName: "Reserva");

            migrationBuilder.RenameIndex(
                name: "IX_Reservas_UsuarioId",
                table: "Reserva",
                newName: "IX_Reserva_UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_Reservas_UsuarioChaDeBebeUsuarioId_UsuarioChaDeBebeChaDeBeb~",
                table: "Reserva",
                newName: "IX_Reserva_UsuarioChaDeBebeUsuarioId_UsuarioChaDeBebeChaDeBebe~");

            migrationBuilder.RenameIndex(
                name: "IX_Reservas_PresenteId",
                table: "Reserva",
                newName: "IX_Reserva_PresenteId");

            migrationBuilder.RenameIndex(
                name: "IX_Reservas_ChaDeBebeEventoId",
                table: "Reserva",
                newName: "IX_Reserva_ChaDeBebeEventoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reserva",
                table: "Reserva",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reserva_ChasDeBebe_ChaDeBebeEventoId",
                table: "Reserva",
                column: "ChaDeBebeEventoId",
                principalTable: "ChasDeBebe",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reserva_Presentes_PresenteId",
                table: "Reserva",
                column: "PresenteId",
                principalTable: "Presentes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reserva_UsuarioChaDeBebe_UsuarioChaDeBebeUsuarioId_UsuarioC~",
                table: "Reserva",
                columns: new[] { "UsuarioChaDeBebeUsuarioId", "UsuarioChaDeBebeChaDeBebeId" },
                principalTable: "UsuarioChaDeBebe",
                principalColumns: new[] { "UsuarioId", "ChaDeBebeId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Reserva_Usuarios_UsuarioId",
                table: "Reserva",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
