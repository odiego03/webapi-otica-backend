using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebApi_otica.Model.Produto
{
    public class ProdutosModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }

        public int? ColecaoId { get; set; }
        public ColecoesModel? Colecao { get; set; }

        
        public List<VariacaoModel> Variacoes { get; set; } = new();
        public DateTime DataCriacao { get; set; } 

        




    }
}
