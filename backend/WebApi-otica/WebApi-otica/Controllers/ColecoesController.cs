using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi_otica.Dto.Colecoes;
using WebApi_otica.Model;
using WebApi_otica.Service;

namespace WebApi_otica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColecoesController : ControllerBase
    {
        private readonly IColecoesService _colecoes;
        private readonly IMapper _mapper;
        public ColecoesController(IColecoesService colecoes, IMapper mapper)
        {
            _colecoes = colecoes;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ColecoesModel>>> GetColecoes()
        {
            try
            {
                var colecoes = await _colecoes.PegarTodasColecoesAsync();

                var resultado = colecoes.Select(c => new ColecaoSimplesDto
                {
                    Id = c.Id,
                    Nome = c.Nome,
                    DescColecao = c.DescColecao,
                    ImagemColecao = c.ImagemColecao,
                    Slug = c.Slug
                }).ToList();

                return Ok(resultado);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao obter coleções: {ex.Message}");
            }
        }

        [HttpGet("{id:int}", Name = "GetColecoesById")]
        public async Task<ActionResult<ColecoesModel>> GetColecoesById(int id)
        {
            try
            {
                var colecao = await _colecoes.PegarColecaoComProdutosEVariaçõesAsync(id);
                if (colecao == null)
                    return NotFound($"Coleção com id {id} não encontrada");

                var resultado = new ColecaoDetalhadaDto
                {
                    Id = colecao.Id,
                    Nome = colecao.Nome,
                    DescColecao = colecao.DescColecao,
                    ImagemColecao = colecao.ImagemColecao,
                    Produtos = colecao.Produtos
                        .SelectMany(p => p.Variacoes.Select(v => new ProdutoDto
                        {
                            VariacaoId = v.Id,
                            ProdutoId = p.Id,
                            NomeProduto = p.Nome,
                            Descricao = p.Descricao,
                            Colecao = colecao.Nome,
                            Cor = v.Cor,
                            Preco = v.Preco,
                            Imagem = v.Imagens.Select(i => i.UrlImagem).FirstOrDefault()

                        }))
                        .ToList()
                };

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao buscar variações da coleção: {ex.Message}");
            }
        }



        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ColecaoSimplesDto>> CriarColecoes([FromForm] CriarColecaoDto colecaoDto)
        {

            
            try
            {
                var colecoes = await _colecoes.CriarColecaoAsync(colecaoDto);
                var resposta = new ColecaoSimplesDto
                { 
                    Id = colecoes.Id,
                    Nome = colecoes.Nome,
                    DescColecao = colecoes.DescColecao,
                    ImagemColecao = colecoes.ImagemColecao

                };
                return CreatedAtRoute(nameof(GetColecoesById), new { id = colecoes.Id }, resposta);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro: {ex.Message}");
            }
        }

        [HttpPatch]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ColecaoSimplesDto>> PatchColecao([FromForm] EditarColecaoDto dto)
        {
            try
            {
                var colecaoExistente = await _colecoes.PegarColecaoPorIdAsync(dto.Id);
                if (colecaoExistente == null)
                    return NotFound($"Coleção com id {dto.Id} não encontrada");

                
               var resposta = await _colecoes.EditarColecaoAsync(dto);

                return Ok(resposta);
            }
            catch
            {
                return BadRequest("Request inválido");
            }

        }

       


        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            try {
                var colecao = await _colecoes.PegarColecaoPorIdAsync(id);
                if (colecao != null)
                {
                    await _colecoes.DeleteColecoesAsync(id);
                    return Ok($"Colecões com id {id} foi excluido");
                }
                else
                {
                    return NotFound($"Coleção com o id {id} não encontrada");

                }


            }
            catch { return BadRequest("Request invalido"); }
        }

        [HttpGet("{slug}")]
        public async Task<IActionResult> GetColecaoBySlug (string slug)
        {
            
           var colecao = await _colecoes.PegarColecaoPorSlug(slug);
            if(colecao == null)
                return NotFound($"Coleção com id {slug} não encontrada");
            var resultado = new ColecaoDetalhadaDto
            {
                Id = colecao.Id,
                Nome = colecao.Nome,
                DescColecao = colecao.DescColecao,
                ImagemColecao = colecao.ImagemColecao,
                Produtos = colecao.Produtos
                       .SelectMany(p => p.Variacoes.Select(v => new ProdutoDto
                       {
                           VariacaoId = v.Id,
                           ProdutoId = p.Id,
                           NomeProduto = p.Nome,
                           Descricao = p.Descricao,
                           Colecao = colecao.Nome,
                           Cor = v.Cor,
                           Preco = v.Preco,
                           Imagem = v.Imagens.Select(i => i.UrlImagem).FirstOrDefault()

                       }))
                       .ToList()
            };

            return Ok(resultado);





        }


    }

  }

