using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.ConstrainedExecution;
using WebApi_otica.Dto.Pruduto.Criar;
using WebApi_otica.Dto.Pruduto.Editar;
using WebApi_otica.Dto.Variacao;
using WebApi_otica.Service.Variacao;
using static System.Net.Mime.MediaTypeNames;

namespace WebApi_otica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VariacaoController : ControllerBase
    {
        private readonly IVariacaoService _variacaoService;

        public VariacaoController(IVariacaoService variacaoService)
        {
            _variacaoService = variacaoService;
        }
       // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("{produtoId}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AdicionarVariacao(int produtoId, [FromForm] CriarVariacaoDto variacaoDto)
        {
            var variacao = await _variacaoService.AdicionarVariacao(produtoId, variacaoDto);
            return Ok(variacao);
           }

        [HttpGet("card-produto")]
        public async Task<IActionResult> ListarVariacoesComUmaImagem()
        {
            try
            {
                var variacoes = await _variacaoService.ListarTodasVariacoes();

                var resultado = variacoes.Select(VariacaoMapper.ParaDto).ToList();
                

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro: {ex.Message}");
            }
        }


        [HttpGet("destaques")]
        public async Task<IActionResult> ListarDestaques()
        {
            try
            {
                var variacoes = await _variacaoService.ListarDestaquesAsync();

                var resultado = variacoes.Select(VariacaoMapper.ParaDto).ToList();

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro: {ex.Message}");
            }
        }

        [HttpGet("{produtoId}/relacionados")]
        public async Task<IActionResult> ListarRelacionados(int produtoId)
        {
            try
            {
                var relacionados = await _variacaoService.ListarRelacionadosAsync(produtoId);

                var resultado = relacionados.Select(VariacaoMapper.ParaDto).ToList();

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro: {ex.Message}");
            }
        }

        [HttpGet("paginado")]
        public async Task<IActionResult> ListarPaginado(
         [FromQuery] int pagina = 1,
         [FromQuery] int tamanho = 10,
         [FromQuery] string? busca = null)
        {
            try
            {
                var paginado = await _variacaoService.ListarPaginadoAsync(pagina, tamanho, busca);

                var variacoesDto = paginado.Resultados.Select(VariacaoMapper.ParaDto).ToList();

                return Ok(new
                {
                    TotalPaginas = paginado.TotalPaginas,
                    PaginaAtual = paginado.PaginaAtual,
                    TamanhoPagina = paginado.TamanhoPagina,
                    TotalResultados = paginado.Total,
                    Variacoes = variacoesDto
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro: {ex.Message}");
            }
        }

        [HttpGet("mais-novos")]
        public async Task<IActionResult> ListarMaisNovos()
        {
            try
            {
                var variacoes = await _variacaoService.ListarMaisNovos();

                var resultado = variacoes.Select(VariacaoMapper.ParaDto).ToList();

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro: {ex.Message}");
            }
        }







        [HttpGet("slug/{slug}")]
        public async Task<IActionResult> GetVariacaoBySlug(string slug)
        {
            try
            {
                var variacao = await _variacaoService.PegarVariacaoPorSlug(slug);
                if (variacao == null)
                    return NotFound($"Não existe essa variação com o slug {slug}");
                var dto = new VariacaoDto
                {
                    VariacaoId = variacao.Id,
                    ProdutoId = variacao.ProdutoId,
                    NomeProduto = variacao.Produto.Nome,
                    Descricao = variacao.Produto.Descricao,
                    Colecao = variacao.Produto.Colecao?.Nome,
                    Cor = variacao.Cor,
                    Slug = variacao.Slug,
                    Preco = variacao.Preco,
                    Imagens = variacao.Imagens.Select(i => i.UrlImagem).ToList(),
                };
            return Ok(dto);
        }
            catch (Exception ex)
            {
                return BadRequest($"Erro: {ex.Message}");
            }

        }


       // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("id{id}")]
        public async Task<IActionResult> GetVariacaoId(int id)
        {
            try
            {
                var variacao = await _variacaoService.PegarVariacaoPorId(id);
               
                if (variacao == null)
                    return NotFound($"Não existe essa variação com o id {id}");
                var dto = VariacaoAdminMapper.ParaDto(variacao);


                return Ok(dto);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro: {ex.Message}");
            }
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPatch("{variacaoId}")]
        public async Task<IActionResult> EditarVariacao(int variacaoId, [FromBody] EditarVariacaoDto variacaoDto)
        {
            var variacao = await _variacaoService.EditarVariacao(variacaoId, variacaoDto);
            return Ok(variacao);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("{variacaoId}")]
        public async Task<IActionResult> DeletarVariacao(int variacaoId)
        {
            var sucesso = await _variacaoService.DeleteVariacao(variacaoId);
            if (!sucesso) return NotFound("Variação não encontrada.");

            return NoContent();
        }

        [HttpGet("colecoes/{idColecao}")]
        public async Task<IActionResult> GetVariacoesByColecao( int idColecao) {
            try
            {
                var variacoes = await _variacaoService.PegarVariacoesPorColecao(idColecao);
                if (variacoes == null)
                    return NotFound($"Não existe essa variação com o id {idColecao}");

                var resultado = variacoes.Select(v => new VariacaoDto
                {
                    VariacaoId = v.Id,
                    ProdutoId = v.ProdutoId,
                    NomeProduto = v.Produto?.Nome,
                    Descricao = v.Produto?.Descricao,
                    Colecao = v.Produto?.Colecao?.Nome,
                    Cor = v.Cor,
                    Slug = v.Slug,
                    Preco = v.Preco,
                    Imagens = new List<string> {
                v.Imagens?.FirstOrDefault()?.UrlImagem ?? ""
            }
                });

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro: {ex.Message}");
            }



        }
    }

}
