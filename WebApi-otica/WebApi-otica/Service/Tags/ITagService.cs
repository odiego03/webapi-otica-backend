using WebApi_otica.Dto.Paginacao;
using WebApi_otica.Model.Produto;

namespace WebApi_otica.Service.Tags
{
    public interface ITagService
    {
        Task<TagModel> CriarTagAsync(string nome);
        Task<PaginacaoResultado<TagModel>> ListarTagsPaginadoAsync(int pagina, int tamanho);
        Task AssociarTagsAVariacaoAsync(int variacaoId, List<string> nomesTags);
        Task DesassociarTagDaVariacaoAsync(int variacaoId, int tagId);
        Task ApagarTagAsync(int tagId);
        Task AtualizarTagAsync(int tagId, string novoNome);
        Task<TagModel> ObterTagPorIdAsync(int tagId);

    }
}
