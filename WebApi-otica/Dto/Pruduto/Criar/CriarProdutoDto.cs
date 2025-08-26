using System.Runtime.ConstrainedExecution;
using System.Text.Json.Serialization;
using WebApi_otica.Model;

namespace WebApi_otica.Dto.Pruduto.Criar
{
    public class CriarProdutoDto
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public int? ColecaoId { get; set; }

        
        public List<CriarVariacaoDto> Variacoes { get; set; }



    }
}
