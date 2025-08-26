using System.Text.Json.Serialization;

namespace WebApi_otica.Dto.Pruduto.Vizualizar
{
    public class VizualizarVariacaoDto
    {

        public int Id { get; set; }
        public string Cor { get; set; }
        public decimal Preco { get; set; }

        
        public List<VizualizarImagemDto> Imagens { get; set; }

    }
}
