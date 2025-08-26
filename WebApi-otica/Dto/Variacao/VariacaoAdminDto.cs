using WebApi_otica.Dto.Pruduto.Vizualizar;
using WebApi_otica.Dto.Tags;

namespace WebApi_otica.Dto.Variacao
{
    public class VariacaoAdminDto
    {
        public int VariacaoId { get; set; }
        public int ProdutoId { get; set; }
        public string NomeProduto { get; set; }
        public string Descricao { get; set; }
        public string Colecao { get; set; }
        public string Cor { get; set; }
        public string Slug { get; set; }
        public decimal Preco { get; set; }
        public List<TagResponseDto> Tags { get; set; }
        public List<VizualizarImagemDto> Imagens { get; set; }
    }
}
