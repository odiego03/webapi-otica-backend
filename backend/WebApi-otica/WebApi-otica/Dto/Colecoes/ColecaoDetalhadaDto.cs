namespace WebApi_otica.Dto.Colecoes
{
    public class ColecaoDetalhadaDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string DescColecao { get; set; }
        public string ImagemColecao { get; set; }
        public List<ProdutoDto> Produtos { get; set; }
    }

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