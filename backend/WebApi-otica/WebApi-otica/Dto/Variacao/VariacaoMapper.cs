using WebApi_otica.Dto.Tags;
using WebApi_otica.Model.Produto;

namespace WebApi_otica.Dto.Variacao
{
    public static class VariacaoMapper
    {
        public static VariacaoDto ParaDto(VariacaoModel v)
        {
            return new VariacaoDto
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
                Imagens = new List<string> {
               v.Imagens?
                 .OrderBy(img => img.Id) 
                 .FirstOrDefault()?.UrlImagem ?? ""

            }
            };
        }
    }

}
