using WebApi_otica.Dto.Paginacao;
using WebApi_otica.Dto.Pruduto.Criar;
using WebApi_otica.Dto.Pruduto.Editar;
using WebApi_otica.Dto.Pruduto.Vizualizar;
using WebApi_otica.Model;
using WebApi_otica.Model.Produto;

namespace WebApi_otica.Service.Variacao
{
    public interface IVariacaoService
    {
        Task<List<VariacaoModel>> MapearVariacoesAsync(List<CriarVariacaoDto> variacoesDto, string nomeProduto);
        Task<VariacaoModel> PegarVariacaoPorId (int  id);

        Task<List<VariacaoModel>> ListarTodasVariacoes();
        Task<VizualizarVariacaoDto> AdicionarVariacao(int produtoId, CriarVariacaoDto variacaoDto);
        Task<bool> DeleteVariacao(int id);
        Task<VizualizarVariacaoDto> EditarVariacao(int variacaoId, EditarVariacaoDto variacaoDto);
        string GerarSlug(params string[] textos);
        Task<VariacaoModel> PegarVariacaoPorSlug(string slug);
        Task<List<VariacaoModel>> ListarMaisNovos();

        Task<List<VariacaoModel>> ListarDestaquesAsync();
        Task<List<VariacaoModel>> ListarRelacionadosAsync(int id);
        Task<PaginacaoResultado<VariacaoModel>> ListarPaginadoAsync(int pagina, int tamanho, string? busca);
        Task<List<VariacaoModel>> PegarVariacoesPorColecao(int idColecao);

    }
}
