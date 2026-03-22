using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChaDeBebe.Api.Migrations
{
    /// <inheritdoc />
    public partial class Produtos2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "ProdutoSequence");

            migrationBuilder.CreateTable(
                name: "Produto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"ProdutoSequence\"')"),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Preco = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produto", x => x.Id);
                });

            // migrationBuilder.CreateTable(
            //     name: "Produtos",
            //     columns: table => new
            //     {
            //         Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"ProdutoSequence\"')"),
            //         Nome = table.Column<string>(type: "text", nullable: false),
            //         Preco = table.Column<decimal>(type: "numeric", nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_Produtos", x => x.Id);
            //     });

            migrationBuilder.CreateTable(
                name: "Produtos2",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"ProdutoSequence\"')"),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Preco = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produtos2", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Produto");

            migrationBuilder.DropTable(
                name: "Produtos");

            migrationBuilder.DropTable(
                name: "Produtos2");

            migrationBuilder.DropSequence(
                name: "ProdutoSequence");
        }
    }
}
