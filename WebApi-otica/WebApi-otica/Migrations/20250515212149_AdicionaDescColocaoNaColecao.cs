using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi_otica.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaDescColocaoNaColecao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DescColocao",
                table: "Colecoes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImagemColecao",
                table: "Colecoes",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescColocao",
                table: "Colecoes");

            migrationBuilder.DropColumn(
                name: "ImagemColecao",
                table: "Colecoes");
        }
    }
}
