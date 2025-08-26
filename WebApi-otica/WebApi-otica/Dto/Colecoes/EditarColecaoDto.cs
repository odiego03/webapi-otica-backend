namespace WebApi_otica.Dto.Colecoes
{
    public class EditarColecaoDto
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? DescColecao { get; set; }
        public IFormFile? fotoColecao { get; set; }
    }
}
