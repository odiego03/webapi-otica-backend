using WebApi_otica.Dto.Paginacao;
using WebApi_otica.Dto.Pruduto;
using WebApi_otica.Dto.Pruduto.Criar;
using WebApi_otica.Dto.Pruduto.Vizualizar;
using WebApi_otica.Model.Produto;

namespace WebApi_otica.Service.Produto
{
    public interface IProdutoService
    {

        //Task<IEnumerable<ProdutosModel>> BuscarProdutoPorNomeAsync(string nome);
        Task<VizualizarProdutoDto> PostProdutoAsync(CriarProdutoDto produtoDTO);
        Task<PaginacaoResultado<ProdutoResumoDto>> GetProdutosPaginado(FiltroPaginacaoDto filtro);
        Task<VizualizarProdutoDto> GetProdutoById(int id);
        Task<VizualizarProdutoDto> PatchProdutoAsync(int id, EditarProdutoDto produtoDto);
        Task<bool> DeleteProdutoAsync(int id);
    }
}
