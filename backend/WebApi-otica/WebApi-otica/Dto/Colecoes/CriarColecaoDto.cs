namespace WebApi_otica.Dto.Colecoes
{
    public class CriarColecaoDto
    {

        public string Nome { get; set; }
        public string DescColecao { get; set; }
        public IFormFile ImagemColecao { get; set; }
    }
}
