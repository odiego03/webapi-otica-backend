using WebApi_otica.Model.Produto;

namespace WebApi_otica.Model
{
    public class ColecoesModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public ICollection<ProdutosModel> Produtos { get; set; } = new List<ProdutosModel>();
        public string Slug { get; set; }
        public string DescColecao { get; set; }
        public string ImagemColecao { get; set; }

    }
}
