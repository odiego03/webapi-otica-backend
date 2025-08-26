using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi_otica.Service.Imagens;

namespace WebApi_otica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagensController : ControllerBase
    {
        private readonly IImagemService _imagemService;

        public ImagensController(IImagemService imagemService)
        {
            _imagemService = imagemService;
        }

        [HttpPost("{variacaoId}/adicionar")]
        public async Task<IActionResult> AdicionarImagens(int variacaoId, [FromForm] List<IFormFile> arquivos)
        {
            Console.WriteLine($"VariacaoId: {variacaoId}");
            Console.WriteLine($"Arquivos recebidos: {arquivos?.Count ?? 0}");
            if (arquivos == null || arquivos.Count == 0)
                return BadRequest("Nenhum arquivo enviado.");

            var imagens = await _imagemService.AddImagemAsync(variacaoId, arquivos);
            return Ok(imagens);
        }

        [HttpDelete("{imagemId}")]
        public async Task<IActionResult> ExcluirImagem(int imagemId)
        {
            var sucesso = await _imagemService.ExcluirImagemAsync(imagemId);
            if (!sucesso) return NotFound("Imagem não encontrada.");

            return NoContent();
        }
    }

}
