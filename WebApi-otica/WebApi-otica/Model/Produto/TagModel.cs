namespace WebApi_otica.Model.Produto
{
    public class TagModel
    {
        public int Id { get; set; }
        public string NomeTag { get; set; }
        public string Slug { get; set; }
        public List<VariacaoModel> Variacoes { get; set; } = new();
    }
}
