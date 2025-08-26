using System.Text.Json.Serialization;

namespace WebApi_otica.Model.Produto
{
    public class VariacaoModel
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public ProdutosModel Produto { get; set; }
        public string Slug { get; set; }
        public string Cor { get; set; }
        public decimal Preco { get; set; }
        public List<TagModel> Tags { get; set; } = new();

        public List<ImagensProdModel>? Imagens { get; set; } = new();
    }
}
