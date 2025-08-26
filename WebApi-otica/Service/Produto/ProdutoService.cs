using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Eventing.Reader;
using WebApi_otica.Data;
using WebApi_otica.Dto.Pruduto.Criar;
using WebApi_otica.Dto.Pruduto.Editar;
using WebApi_otica.Dto.Pruduto.Vizualizar;
using WebApi_otica.Model.Produto;
using WebApi_otica.Service.Variacao;
using WebApi_otica.Service.Imagens;
using WebApi_otica.Dto.Paginacao;
using WebApi_otica.Dto.Pruduto;

namespace WebApi_otica.Service.Produto
{
    public class ProdutoService : IProdutoService
    {

        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IVariacaoService _variacaoService;
        private readonly IImagemService _imagemService;



        public ProdutoService(AppDbContext context, IMapper mapper, IVariacaoService variacaoService, IImagemService imagemService)
        {
            _context = context;
            _mapper = mapper;
            _variacaoService = variacaoService;
            _imagemService = imagemService;
        }



        public async Task<VizualizarProdutoDto> PostProdutoAsync(CriarProdutoDto produtoDTO)
        {
            if (produtoDTO.ColecaoId == 0)
            { produtoDTO.ColecaoId = null; } // Certifique-se que a coleção será nula se não fornecida

            if (produtoDTO.ColecaoId != null)
            {
                //verificar se colecao existe
                var colecao = await _context.Colecoes.FindAsync(produtoDTO.ColecaoId);
                if (colecao == null)
                {
                    throw new ArgumentException("Coleção não encontrada.");
                }

            }

            var produtoMapeado = _mapper.Map<ProdutosModel>(produtoDTO);
            produtoMapeado.DataCriacao = DateTime.UtcNow;
            var variacoesMapeadas = await _variacaoService.MapearVariacoesAsync(produtoDTO.Variacoes, produtoDTO.Nome);
            produtoMapeado.Variacoes = variacoesMapeadas;
            await _context.Produtos.AddAsync(produtoMapeado);
            await _context.SaveChangesAsync();

            var produtoCompleto = await _context.Produtos.Include(p => p.Colecao).Include(p => p.Variacoes).ThenInclude(v => v.Imagens).FirstOrDefaultAsync(p => p.Id == produtoMapeado.Id);
            if (produtoCompleto == null)
                throw new Exception("Erro ao buscar produto");

            var retorno = _mapper.Map<VizualizarProdutoDto>(produtoCompleto);
            return retorno;
        }



        public async Task<bool> DeleteProdutoAsync(int id)
        {
            var produto = await _context.Produtos.Include(p => p.Variacoes)
                                                 .ThenInclude(v => v.Imagens)
                                                 .FirstOrDefaultAsync(p => p.Id == id);
            if (produto != null)
            {
                foreach (var variacao in produto.Variacoes)
                {
                    // Remove as imagens associadas à variação
                    if (variacao.Imagens != null && variacao.Imagens.Count > 0)
                    {
                        foreach (var imagem in variacao.Imagens)
                        {
                           await _imagemService.ExcluirImagemFisicaAsync(imagem.UrlImagem);
                        }
                        _context.ImagensProdutos.RemoveRange(variacao.Imagens);
                    }
                }
                // Remove as variações associadas ao produto
                _context.VariacaoProdutos.RemoveRange(produto.Variacoes);
                // Remove o produto
                _context.Produtos.Remove(produto);
                await _context.SaveChangesAsync();
                return true;

            }
            return false;
        }

        public async Task<VizualizarProdutoDto> PatchProdutoAsync(int id,EditarProdutoDto produtoDto)
        {
            
            var produto = await _context.Produtos.Include(p => p.Colecao)
                .Include(p => p.Variacoes)
                .ThenInclude(v => v.Imagens)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (produto == null)
                throw new ArgumentException("Produto não encontrado para edição.");

            if (produtoDto.Nome != null) { 
                produto.Nome = produtoDto.Nome;
                // Atualiza o slug com base no novo nome
                foreach (var variacao in produto.Variacoes)
                {
                    variacao.Slug = _variacaoService.GerarSlug(produto.Nome, variacao.Cor);
                }
            }
            if (produtoDto.Descricao != null) produto.Descricao = produtoDto.Descricao;
            if(produtoDto.ColecaoId != null) produto.ColecaoId = produtoDto.ColecaoId;

            
            await _context.SaveChangesAsync();
            var produtoAtualizado = await _context.Produtos
                    .Include(p => p.Variacoes).ThenInclude(v => v.Imagens)
                    .Include(p => p.Colecao)
                    .FirstOrDefaultAsync(p => p.Id == id);

            return _mapper.Map<VizualizarProdutoDto>(produtoAtualizado);
        }

        public async Task<VizualizarProdutoDto> GetProdutoById(int id)
        {
            var produto = await _context.Produtos.Include(p => p.Colecao).Include(p => p.Variacoes)
                .ThenInclude(v => v.Imagens).AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
            if (produto == null)
                throw new Exception("Produto não encontrado");
            return _mapper.Map<VizualizarProdutoDto>(produto);
        }


        public async Task<PaginacaoResultado<ProdutoResumoDto>> GetProdutosPaginado(FiltroPaginacaoDto filtro)
        {
            var query = _context.Produtos
                .Include(p => p.Colecao)
                .AsNoTracking();

            var total = await query.CountAsync();
            var totalPaginas = (int)Math.Ceiling(total / (double)filtro.TamanhoPagina);

            var produtos = await query
                .OrderByDescending(p => p.DataCriacao)
                .Skip((filtro.Pagina - 1) * filtro.TamanhoPagina)
                .Take(filtro.TamanhoPagina)
                .Select(p => new ProdutoResumoDto
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    NomeColecao = p.Colecao.Nome,
                    DataCriacao = p.DataCriacao
                })
                .ToListAsync();

            return new PaginacaoResultado<ProdutoResumoDto>
            {
                Total = total,
                PaginaAtual = filtro.Pagina,
                TamanhoPagina = filtro.TamanhoPagina,
                TotalPaginas = totalPaginas,
                Resultados = produtos
            };
        }


    }
}







