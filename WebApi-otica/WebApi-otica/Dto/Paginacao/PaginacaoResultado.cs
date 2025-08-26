namespace WebApi_otica.Dto.Paginacao
{
    public class PaginacaoResultado<T>
    {
        public int Total { get; set; }
        public int PaginaAtual { get; set; }
        public int TamanhoPagina { get; set; }
        public int TotalPaginas { get; set; }
        public List<T> Resultados { get; set; }
    }
}
