using WebApi_otica.Dto.Colecoes;
using WebApi_otica.Model;

namespace WebApi_otica.Service
{
    public interface IColecoesService
    {
        Task<IEnumerable<ColecoesModel>> PegarTodasColecoesAsync();
        Task<ColecoesModel> PegarColecaoPorIdAsync(int id);
        Task<ColecoesModel> CriarColecaoAsync(CriarColecaoDto colecaoDto);
        Task<ColecaoSimplesDto> EditarColecaoAsync( EditarColecaoDto colecoes);
        Task<bool> DeleteColecoesAsync(int id);
        Task<ColecoesModel?> PegarColecaoComProdutosEVariaçõesAsync(int id);
        Task<bool> ExcluirImgColecao(string url);
        Task<string> SalvarImgColecao(IFormFile foto);
        Task<ColecoesModel> PegarColecaoPorSlug(string slug);

    }
}
