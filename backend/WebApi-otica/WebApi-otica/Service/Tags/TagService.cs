using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using WebApi_otica.Data;
using WebApi_otica.Dto.Paginacao;
using WebApi_otica.Model.Produto;

namespace WebApi_otica.Service.Tags
{
    public class TagService : ITagService
    {
        private readonly AppDbContext _context;

        public TagService(AppDbContext context)
        {
            _context = context;
        }

        private string GerarSlug(string nome)
        {
            var normalized = nome.ToLowerInvariant().Trim();
            normalized = normalized.Normalize(NormalizationForm.FormD);
            var chars = normalized.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark);
            var clean = new string(chars.ToArray());
            clean = Regex.Replace(clean, @"[^a-z0-9\s-]", ""); // remove caracteres especiais
            clean = Regex.Replace(clean, @"\s+", "-"); // espaços por hífen
            return clean.Trim('-');
        }

        public async Task<TagModel> CriarTagAsync(string nome)
        {
            var slug = GerarSlug(nome);

            var tagExistente = await _context.Tags.FirstOrDefaultAsync(t => t.Slug == slug);
            if (tagExistente != null)
            {
                return tagExistente;
            }

            var novaTag = new TagModel
            {
                NomeTag = nome.Trim(),
                Slug = slug
            };

            _context.Tags.Add(novaTag);
            await _context.SaveChangesAsync();

            return novaTag;
        }

        public async Task<PaginacaoResultado<TagModel>> ListarTagsPaginadoAsync(int pagina, int tamanho)
        {
            var query = _context.Tags.OrderBy(t => t.NomeTag);

            var total = await query.CountAsync();

            var resultados = await query
                .Skip((pagina - 1) * tamanho)
                .Take(tamanho)
                .ToListAsync();

            var resultadoPaginado = new PaginacaoResultado<TagModel>
            {
                Total = total,
                PaginaAtual = pagina,
                TamanhoPagina = tamanho,
                TotalPaginas = (int)Math.Ceiling(total / (double)tamanho),
                Resultados = resultados
            };

            return resultadoPaginado;
        }


        public async Task ApagarTagAsync(int tagId)
        {
            var tag = await _context.Tags
                .Include(t => t.Variacoes)
                .FirstOrDefaultAsync(t => t.Id == tagId);

            if (tag == null)
                throw new Exception("Tag não encontrada.");

            tag.Variacoes.Clear(); // desassocia de todas as variações
            _context.Tags.Remove(tag);

            await _context.SaveChangesAsync();
        }

        

        public async Task AssociarTagsAVariacaoAsync(int variacaoId, List<string> nomesTags)
        {
            var variacao = await _context.VariacaoProdutos
                .Include(v => v.Tags)
                .FirstOrDefaultAsync(v => v.Id == variacaoId);

            if (variacao == null)
                throw new Exception("Variação não encontrada.");

            foreach (var nome in nomesTags)
            {
                var tag = await CriarTagAsync(nome); // cria ou retorna existente
                if (!variacao.Tags.Any(t => t.Id == tag.Id))
                {
                    variacao.Tags.Add(tag);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task DesassociarTagDaVariacaoAsync(int variacaoId, int tagId)
        {
            var variacao = await _context.VariacaoProdutos
                .Include(v => v.Tags)
                .FirstOrDefaultAsync(v => v.Id == variacaoId);

            if (variacao == null)
                throw new Exception("Variação não encontrada.");

            var tag = variacao.Tags.FirstOrDefault(t => t.Id == tagId);
            if (tag != null)
            {
                variacao.Tags.Remove(tag);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AtualizarTagAsync(int tagId, string novoNome)
        {
            var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Id == tagId);

            if (tag == null)
                throw new Exception("Tag não encontrada.");

            var novoSlug = GerarSlug(novoNome);

            // Verifica se já existe outra tag com o mesmo slug
            var slugExistente = await _context.Tags
                .AnyAsync(t => t.Slug == novoSlug && t.Id != tagId);

            if (slugExistente)
                throw new Exception("Já existe outra tag com esse nome.");

            tag.NomeTag = novoNome.Trim();
            tag.Slug = novoSlug;

            _context.Tags.Update(tag);
            await _context.SaveChangesAsync();
        }

        public async Task<TagModel> ObterTagPorIdAsync(int tagId)
        {
            var tag = await _context.Tags
                .FirstOrDefaultAsync(t => t.Id == tagId);

            if (tag == null)
                throw new Exception("Tag não encontrada.");

            return tag;
        }

    }
}
