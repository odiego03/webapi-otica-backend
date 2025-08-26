namespace WebApi_otica.Dto.Pruduto.Editar
{
    public class EditarImagemDto
    {
        public int? Id { get; set; } // Se for null, é nova
        public string UrlImagem { get; set; }
    }
}
