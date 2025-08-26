using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi_otica.Migrations
{
    /// <inheritdoc />
    public partial class RenomearDescColocaoParaDescColecao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DescColocao",
                table: "Colecoes",
                newName: "DescColecao");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DescColecao",
                table: "Colecoes",
                newName: "DescColocao");
        }
    }
}
