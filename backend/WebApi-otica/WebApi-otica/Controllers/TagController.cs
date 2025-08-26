using Microsoft.AspNetCore.Mvc;
using WebApi_otica.Dto.Tags;
using WebApi_otica.Service.Tags;

namespace WebApi_otica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly ITagService _tagService;

        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }

        [HttpPost]
        public async Task<ActionResult<TagResponseDto>> CriarTag([FromBody] TagCreateDTO dto)
        {
            var tag = await _tagService.CriarTagAsync(dto.NomeTag);

            var response = new TagResponseDto
            {
                Id = tag.Id,
                NomeTag = tag.NomeTag,
                Slug = tag.Slug
            };

            return CreatedAtAction(nameof(ObterTagPorId), new { tagId = tag.Id }, response);
        }

        [HttpGet("paginado")]
        public async Task<IActionResult> ListarPaginado([FromQuery] int pagina = 1, [FromQuery] int tamanho = 10)
        {
            try
            {
                var paginado = await _tagService.ListarTagsPaginadoAsync(pagina, tamanho);

                var tagsDto = paginado.Resultados.Select(tag => new TagResponseDto
                {
                    Id = tag.Id,
                    NomeTag = tag.NomeTag,
                    Slug = tag.Slug
                }).ToList();

                return Ok(new
                {
                    TotalPaginas = paginado.TotalPaginas,
                    PaginaAtual = paginado.PaginaAtual,
                    TamanhoPagina = paginado.TamanhoPagina,
                    TotalResultados = paginado.Total,
                    Tags = tagsDto
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro: {ex.Message}");
            }
        }


        [HttpGet("{tagId}")]
        public async Task<ActionResult<TagResponseDto>> ObterTagPorId(int tagId)
        {
            var tag = await _tagService.ObterTagPorIdAsync(tagId);
            if (tag == null)
                return NotFound(new { mensagem = "Tag não encontrada." });

            return Ok(new TagResponseDto
            {
                Id = tag.Id,
                NomeTag = tag.NomeTag,
                Slug = tag.Slug
            });
        }

        [HttpPost("{variacaoId}/associar")]
        public async Task<IActionResult> AssociarTags(int variacaoId, [FromBody] TagAssociarDto dto)
        {
            await _tagService.AssociarTagsAVariacaoAsync(variacaoId, dto.NomesTags);
            return Ok(new { mensagem = "Tags associadas com sucesso." });
        }

        [HttpDelete("{variacaoId}/desassociar/{tagId}")]
        public async Task<IActionResult> DesassociarTag(int variacaoId, int tagId)
        {
            await _tagService.DesassociarTagDaVariacaoAsync(variacaoId, tagId);
            return Ok(new { mensagem = "Tag desassociada com sucesso." });
        }

        [HttpDelete("{tagId}")]
        public async Task<IActionResult> ApagarTag(int tagId)
        {
            await _tagService.ApagarTagAsync(tagId);
            return Ok(new { mensagem = "Tag apagada com sucesso." });
        }

        [HttpPut("{tagId}")]
        public async Task<IActionResult> AtualizarTag(int tagId, [FromBody] TagCreateDTO dto)
        {
            await _tagService.AtualizarTagAsync(tagId, dto.NomeTag);
            return Ok(new { mensagem = "Tag atualizada com sucesso." });
        }
    }
}
