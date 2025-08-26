namespace WebApi_otica.Dto.Pruduto.Criar
{
    public class CriarVariacaoDto
    {
       
        public string Cor { get; set; }
        public decimal Preco { get; set; }

        public List<IFormFile> Imagens { get; set; }

    }
}
