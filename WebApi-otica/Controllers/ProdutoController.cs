using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi_otica.Dto.Paginacao;
using WebApi_otica.Dto.Pruduto.Criar;
using WebApi_otica.Model.Produto;
using WebApi_otica.Service.Imagens;
using WebApi_otica.Service.Produto;
using WebApi_otica.Service.Variacao;

namespace WebApi_otica.Controllers
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoController : ControllerBase
    {
        private readonly IProdutoService _produtoService;
        private readonly IVariacaoService _variacaoService;
        private readonly IImagemService _imagemService;


        public ProdutoController(IProdutoService produtoService, IImagemService imagemService, IVariacaoService variacaoService)
        {
            _produtoService = produtoService;
            _imagemService = imagemService;
            _variacaoService = variacaoService;
        }


        [HttpGet]
        public async Task<IActionResult> GetTodosProdutos([FromQuery] FiltroPaginacaoDto filtro )
        {
            try
            {
                var resultado = await _produtoService.GetProdutosPaginado(filtro);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao obter produtos: {ex.Message}");
            }
        }



        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CriarProduto([FromForm] CriarProdutoDto produtoDto)
        {
            try
            {

                var produtoCriado = await _produtoService.PostProdutoAsync(produtoDto);
                return CreatedAtRoute(nameof(GetProdutoPorId), new { id = produtoCriado.Id }, produtoCriado);

            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Erro: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao criar produto: {ex.Message}");
            }
        }

        [HttpGet("{id}", Name = "GetProdutoPorId")]
        public async Task<IActionResult> GetProdutoPorId(int id)
        {
            try
            {
                var produto = await _produtoService.GetProdutoById(id);
                if (produto == null)
                    return NotFound($"Produto com ID {id} não encontrado.");
                return Ok(produto);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar produto: {ex.Message}");
            }
        }


        [HttpPatch("{id}")]
        public async Task<IActionResult> EditarProduto(int id, [FromBody] EditarProdutoDto produtoDto)
        {
            try
            {
                if (produtoDto.Id != id)
                {
                    return BadRequest("Dados inconsistentes");
                }

                // Verifica se ColecaoId é nulo, caso sim, então zera a Colecao
                if (produtoDto.ColecaoId == null)
                {
                    produtoDto.ColecaoId = null;
                }

                var produtoEditado = await _produtoService.PatchProdutoAsync(id, produtoDto);
                if (produtoEditado == null)
                {
                    return NotFound($"Produto com ID {id} não encontrado.");
                }

                return Ok($"Produto com ID {id} foi alterado com sucesso.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Erro: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao editar produto: {ex.Message}");
            }
        }


        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeletarProduto(int id)
        {
            try
            {
                var produto = await _produtoService.GetProdutoById(id);
                if (produto == null)
                {
                    return NotFound($"Produto com ID {id} não encontrado.");
                }

                await _produtoService.DeleteProdutoAsync(id);
                return Ok($"Produto com ID {id} foi excluído com sucesso.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao deletar produto: {ex.Message}");
            }
        }


        
        


    }
}
