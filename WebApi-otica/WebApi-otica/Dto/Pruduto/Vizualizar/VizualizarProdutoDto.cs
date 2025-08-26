using System.Text.Json.Serialization;

namespace WebApi_otica.Dto.Pruduto.Vizualizar
{
    public class VizualizarProdutoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string? NomeColecao { get; set; }
        public DateTime DataCriacao { get; set; }
        
        public List<VizualizarVariacaoDto> Variacoes { get; set; }
    }





}
