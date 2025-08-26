namespace WebApi_otica.Dto.Pruduto
{
    public class ProdutoDto
    {
        public int VariacaoId { get; set; }
        public int ProdutoId { get; set; }
        public string NomeProduto { get; set; }
        public string Descricao { get; set; }
        public string Colecao { get; set; }
        public string Cor { get; set; }
        public decimal Preco { get; set; }
        public string Imagem { get; set; }
    }
}
