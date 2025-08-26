using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebApi_otica.Data;
using WebApi_otica.Dto.Paginacao;
using WebApi_otica.Dto.Pruduto.Criar;
using WebApi_otica.Dto.Pruduto.Editar;
using WebApi_otica.Dto.Pruduto.Vizualizar;
using WebApi_otica.Model;
using WebApi_otica.Model.Produto;
using WebApi_otica.Service.Imagens;

namespace WebApi_otica.Service.Variacao
{
    public class VariacaoService : IVariacaoService
    {
        private readonly IImagemService _imagemService;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;

        public VariacaoService(IMapper mapper,IImagemService imagemService, AppDbContext context)
        {
            _imagemService = imagemService;
            _mapper = mapper;
            _context = context;
        }

        public async Task<List<VariacaoModel>> MapearVariacoesAsync(List<CriarVariacaoDto> variacoesDto, string nomeProduto)
        {
            var variacoes = new List<VariacaoModel>();

            foreach (var variacaoDto in variacoesDto)
            {

                var variacaoMapeada = _mapper.Map<VariacaoModel>(variacaoDto);
                variacaoMapeada.Slug = GerarSlug(nomeProduto,variacaoDto.Cor);
                

                variacaoMapeada.Imagens = new List<ImagensProdModel>();
                if (variacaoDto.Imagens != null && variacaoDto.Imagens.Count > 0)
                {
                    foreach (var imagemDto in variacaoDto.Imagens)
                    {
                        //aqui eu mando uma imagem pra salvar na pasta chamando um metodo
                        //q vai retornar uma string da url dessa img
                        var imagemUrl = await _imagemService.SalvarImagemAsync(imagemDto);

                        //o codigo aqui manda a url para uma nova imagensProdModel
                        var imagemMapeada = new ImagensProdModel
                        {
                            UrlImagem = imagemUrl.UrlImagem 
                        };
                        //manda essa imagem  criada para a variacao mapeada
                        variacaoMapeada.Imagens.Add(imagemMapeada);
                    }
                }
                // aqui eu adiciono a variacao mapeada na lista de variacoes
                variacoes.Add(variacaoMapeada);
            }
            //e por fim retorno uma lista de variacoes
            return variacoes;
        }




        public async Task<VizualizarVariacaoDto> AdicionarVariacao(int produtoId, CriarVariacaoDto variacaoDto)
        {
            var produto = await _context.Produtos
                .Include(p => p.Variacoes)
                .FirstOrDefaultAsync(p => p.Id == produtoId);

            if (produto == null) throw new ArgumentException("Produto não encontrado.");

            var variacaoMapeada = _mapper.Map<VariacaoModel>(variacaoDto);
            variacaoMapeada.ProdutoId = produtoId;
            variacaoMapeada.Slug = GerarSlug(produto.Nome,variacaoDto.Cor);
            variacaoMapeada.Imagens = new List<ImagensProdModel>();

            if (variacaoDto.Imagens != null && variacaoDto.Imagens.Count > 0)
            {
                foreach (var imagemDto in variacaoDto.Imagens)
                {
                    var imagemUrl = await _imagemService.SalvarImagemAsync(imagemDto);
                    var imagemMapeada = new ImagensProdModel
                    {
                        UrlImagem = imagemUrl.UrlImagem
                    };
                    variacaoMapeada.Imagens.Add(imagemMapeada);
                }
            }

            // 🔧 Adicione à lista de variações do produto
            produto.Variacoes.Add(variacaoMapeada);

            // 🔧 Adicione ao contexto e salve
            _context.VariacaoProdutos.Add(variacaoMapeada);
            await _context.SaveChangesAsync();
            var retorno = _mapper.Map<VizualizarVariacaoDto>(variacaoMapeada);
            return retorno;
        }



        public async Task<bool> DeleteVariacao(int id)
        {
            var variacao = await _context.VariacaoProdutos
                .Include(v => v.Imagens)
                .FirstOrDefaultAsync(v => v.Id == id);
            if (variacao == null) return false;
            var imagens = variacao.Imagens;
            if (imagens != null && imagens.Count > 0)
            { 
                foreach(var imagem in imagens)
                {
                    await _imagemService.ExcluirImagemFisicaAsync(imagem.UrlImagem);

                }
                _context.ImagensProdutos.RemoveRange(imagens);
            }
            _context.VariacaoProdutos.Remove(variacao);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<VizualizarVariacaoDto> EditarVariacao(int variacaoId, EditarVariacaoDto variacaoDto)
        {
            var variacao = await _context.VariacaoProdutos
                .Include(v => v.Imagens)
                .Include(v => v.Produto)
                .FirstOrDefaultAsync(v => v.Id == variacaoId);
            if (variacao == null) throw new ArgumentException("Variação não encontrada.");

            if (variacaoDto.Cor != null)
            {
                variacao.Cor = variacaoDto.Cor;
                if (variacao.Produto != null)
                    variacao.Slug = GerarSlug(variacao.Produto.Nome, variacaoDto.Cor);
                else
                    variacao.Slug = GerarSlug(variacaoDto.Cor);
            }

            if (variacaoDto.Preco != null)
                variacao.Preco = variacaoDto.Preco.Value;

            // Salva as alterações no banco
            await _context.SaveChangesAsync();

            // Agora mapeia a entidade atualizada para o DTO de visualização
            var retorno = _mapper.Map<VizualizarVariacaoDto>(variacao);
            return retorno;
        }


        public async Task<PaginacaoResultado<VariacaoModel>> ListarPaginadoAsync(int pagina, int tamanho, string? busca)
        {
            var query = _context.VariacaoProdutos
                .Include(v => v.Produto)
                .Include(v => v.Tags)
                .Include(v => v.Imagens)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(busca))
            {
                var termo = busca.ToLower();

                query = query.Where(v =>
                    v.Produto.Nome.ToLower().Contains(termo) ||
                    v.Produto.Descricao.ToLower().Contains(termo) ||
                    v.Cor.ToLower().Contains(termo) ||
                    v.Tags.Any(tag => tag.NomeTag.ToLower().Contains(termo))
                );
            }

            var total = await query.CountAsync();
            var totalPaginas = (int)Math.Ceiling(total / (double)tamanho);

            if (pagina > totalPaginas && totalPaginas > 0)
                pagina = totalPaginas;

            var variacoes = await query
                .Skip((pagina - 1) * tamanho)
                .Take(tamanho)
                .ToListAsync();

            return new PaginacaoResultado<VariacaoModel>
            {
                Total = total,
                PaginaAtual = pagina,
                TamanhoPagina = tamanho,
                TotalPaginas = totalPaginas,
                Resultados = variacoes
            };
        }




        public async Task<List<VariacaoModel>> ListarDestaquesAsync()
        {
            return await _context.VariacaoProdutos
                .Include(v => v.Produto)
                .Include(v => v.Tags)
                .Include(v => v.Imagens)
                .Where(v => v.Tags.Any(t => t.Slug == "destaque"))
                .OrderByDescending(v => v.Produto.DataCriacao) 
                .Take(12)
                .ToListAsync();
        }
        public async Task<List<VariacaoModel>> ListarRelacionadosAsync(int id)
        {
            var variacao = await _context.VariacaoProdutos
                .Include(v => v.Produto)
                    .ThenInclude(p => p.Colecao)
                .Include(v => v.Tags)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (variacao == null) throw new Exception("Produto não encontrado");

            var nomeProduto = variacao.Produto?.Nome;
            var colecaoId = variacao.Produto?.Colecao?.Id;
            var tagIds = variacao.Tags.Select(t => t.Id).ToList();

            var candidatos = await _context.VariacaoProdutos
                .Include(v => v.Produto)
                    .ThenInclude(p => p.Colecao)
                .Include(v => v.Imagens)
                .Include(v => v.Tags)
                .Where(v => v.Id != id)
                .ToListAsync();

            // Relevância: 3 pontos se nome e coleção batem, 2 se só nome, 1 se tem tag em comum
            var relacionados = candidatos
                .Select(v =>
                {
                    int score = 0;

                    if (v.Produto?.Nome == nomeProduto)
                    {
                        score += 1;
                        if (v.Produto?.Colecao?.Id == colecaoId)
                            score += 2; // total 3 se nome + coleção iguais
                    }

                    int tagsEmComum = v.Tags.Count(t => tagIds.Contains(t.Id));
                    if (tagsEmComum > 0)
                        score += 1;

                    return new { Variacao = v, Score = score, TagsMatch = tagsEmComum };
                })
                .Where(x => x.Score > 0)
                .OrderByDescending(x => x.Score)
                .ThenByDescending(x => x.TagsMatch)
                .Take(10)
                .Select(x => x.Variacao)
                .ToList();

            return relacionados;
        }


        public async Task<List<VariacaoModel>> ListarTodasVariacoes()
        {
            return await _context.VariacaoProdutos
            .Include(v => v.Produto)
                .ThenInclude(p => p.Colecao)
            .Include(v => v.Tags) 
            .Include(v => v.Imagens)
            .ToListAsync();
        }


        public async Task<VariacaoModel> PegarVariacaoPorId(int id)
        {
            try
            {
                var variacao = await _context.VariacaoProdutos.
                    Include(v => v.Produto)
                    .ThenInclude(p => p.Colecao)
                    .Include(v => v.Tags)
                    .Include(v => v.Imagens)
                    .FirstOrDefaultAsync(v => v.Id == id);
                return variacao;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar variação por ID", ex);
            }
        }

        public async Task<List<VariacaoModel>> ListarMaisNovos()
        {
            return await _context.VariacaoProdutos
                .Include(v => v.Produto)
                .Include(v => v.Tags)
                .Include(v => v.Imagens)
                .OrderByDescending(v => v.Produto.DataCriacao)
                .Take(10)
                .ToListAsync();
        }

        public string GerarSlug(params string[] textos)
        {
            var combinado = string.Join("-", textos).ToLower().Trim();

            var slug = combinado
               .Replace(" ", "-")
               .Replace("ç", "c")
               .Replace("ã", "a")
               .Replace("õ", "o")
               .Replace("á", "a")
               .Replace("é", "e")
               .Replace("í", "i")
               .Replace("ó", "o")
               .Replace("ú", "u")
               .Replace("â", "a")
               .Replace("ê", "e")
               .Replace("ô", "o")
               .Replace("?", "")
               .Replace(",", "")
               .Replace(".", "")
               .Replace("/", "-");

            return slug;
        }




        public async Task<VariacaoModel> PegarVariacaoPorSlug(string slug)
        {
            try {
                var variacao = await _context.VariacaoProdutos
                    .Include(v => v.Produto)
                    .ThenInclude(p => p.Colecao)
                    .Include(v => v.Imagens)
                    .FirstOrDefaultAsync(v => v.Slug == slug);
                return variacao;

            }
            catch(Exception ex)
            {
                throw new Exception("Erro ao buscar variação por slug", ex);

            }
        }


        public async Task<List<VariacaoModel>> PegarVariacoesPorColecao(int idColecao)
        {
            return await _context.VariacaoProdutos.
                 Include(v => v.Produto)
                    .ThenInclude(p => p.Colecao)
                    .Include(v => v.Imagens)
                .Where(v => v.Produto.ColecaoId == idColecao)
                .ToListAsync();
        }

    }




    
   }

