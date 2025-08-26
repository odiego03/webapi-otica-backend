using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi_otica.Migrations
{
    /// <inheritdoc />
    public partial class MoverTagsParaVariacao2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProdutoTags");

            migrationBuilder.CreateTable(
                name: "VariacaoTags",
                columns: table => new
                {
                    TagsId = table.Column<int>(type: "integer", nullable: false),
                    VariacoesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VariacaoTags", x => new { x.TagsId, x.VariacoesId });
                    table.ForeignKey(
                        name: "FK_VariacaoTags_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VariacaoTags_VariacaoProdutos_VariacoesId",
                        column: x => x.VariacoesId,
                        principalTable: "VariacaoProdutos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VariacaoTags_VariacoesId",
                table: "VariacaoTags",
                column: "VariacoesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VariacaoTags");

            migrationBuilder.CreateTable(
                name: "ProdutoTags",
                columns: table => new
                {
                    ProdutosId = table.Column<int>(type: "integer", nullable: false),
                    TagsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProdutoTags", x => new { x.ProdutosId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_ProdutoTags_Produtos_ProdutosId",
                        column: x => x.ProdutosId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProdutoTags_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProdutoTags_TagsId",
                table: "ProdutoTags",
                column: "TagsId");
        }
    }
}
