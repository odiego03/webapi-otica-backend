using WebApi_otica.Dto.Pruduto.Vizualizar;

namespace WebApi_otica.Service.Imagens
{
    public interface IImagemService
    {
        Task<ImagensProdModel> SalvarImagemAsync(IFormFile imagem);
        Task ExcluirImagemFisicaAsync(string urlImagem);
        Task<List<VizualizarImagemDto>> AddImagemAsync(int variacaoId, List<IFormFile> arquivos);

        Task<bool> ExcluirImagemAsync(int imagemId);
    }
}
