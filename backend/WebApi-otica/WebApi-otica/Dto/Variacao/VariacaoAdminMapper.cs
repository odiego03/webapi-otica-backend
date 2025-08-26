using WebApi_otica.Model.Produto;
using WebApi_otica.Dto.Tags;
using WebApi_otica.Dto.Pruduto.Vizualizar;

namespace WebApi_otica.Dto.Variacao
{
    public static class VariacaoAdminMapper
    {
        public static VariacaoAdminDto ParaDto(VariacaoModel v)
        {
            return new VariacaoAdminDto
            {
                VariacaoId = v.Id,
                ProdutoId = v.ProdutoId,
                NomeProduto = v.Produto?.Nome,
                Descricao = v.Produto?.Descricao,
                Colecao = v.Produto?.Colecao?.Nome,
                Cor = v.Cor,
                Slug = v.Slug,
                Preco = v.Preco,
                Tags = v.Tags?.Select(t => new TagResponseDto
                {
                    Id = t.Id,
                    NomeTag = t.NomeTag
                }).ToList() ?? new List<TagResponseDto>(),

                Imagens = v.Imagens?.Select(img => new VizualizarImagemDto
                {
                    Id = img.Id,
                    UrlImagem = img.UrlImagem
                }).ToList() ?? new List<VizualizarImagemDto>()
            };
        }
    }
}
