using AutoMapper;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using WebApi_otica.Data;
using WebApi_otica.Dto.Colecoes;
using WebApi_otica.Model;
using WebApi_otica.Model.Produto;
using Amazon.S3;
using Amazon.S3.Model;

namespace WebApi_otica.Service
{
    public class ColecoesService : IColecoesService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAmazonS3 _s3Client; 
        private const string BucketName = "hk-ecommerce-imagens"; // Nome do bucket

        public ColecoesService(AppDbContext context, IMapper mapper, IAmazonS3 s3Client) 
        {
            _context = context;
            _mapper = mapper;
            _s3Client = s3Client; // Injete
        }
        public async Task<ColecoesModel> CriarColecaoAsync(CriarColecaoDto colecaoDto)
        {



            var tiposPermitidos = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var tamanhoMaximo = 5 * 1024 * 1024;// 5 MB
            if (colecaoDto.ImagemColecao != null)
            {
                if (!tiposPermitidos.Contains(Path.GetExtension(colecaoDto.ImagemColecao.FileName).ToLower()))
                {
                    throw new Exception("Tipo de arquivo não permitido. Apenas JPG, JPEG, PNG e WEBP são aceitos.");
                }
                if (colecaoDto.ImagemColecao.Length > tamanhoMaximo)
                {
                    throw new Exception("O tamanho do arquivo excede o limite de 5 MB.");
                }


            }
            var colecao = _mapper.Map<ColecoesModel>(colecaoDto);
            var url = await SalvarImgColecao(colecaoDto.ImagemColecao);
            colecao.Slug = GerarSlug(colecao.Nome);


            colecao.ImagemColecao = url;
            _context.Colecoes.Add(colecao);
            await _context.SaveChangesAsync();
            return colecao;
        }


        public async Task<string> SalvarImgColecao(IFormFile foto)
        {
            
            var nomeArq = Guid.NewGuid() + Path.GetExtension(foto.FileName);
            
            var caminhoNoBucket = $"colecoes/{nomeArq}";

            try
            {
                // Prepara e executa o upload para o S3
                var request = new PutObjectRequest
                {
                    BucketName = BucketName,
                    Key = caminhoNoBucket,
                    InputStream = foto.OpenReadStream(),
                    ContentType = foto.ContentType
                };

                await _s3Client.PutObjectAsync(request);

                // Gera a URL pública da imagem no S3
                var urlPublica = $"https://{BucketName}.s3.sa-east-1.amazonaws.com/{caminhoNoBucket}";
                return urlPublica; // Retorna a URL completa do S3
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao salvar imagem da coleção no S3: {ex.Message}");
            }
        }
        public async Task<bool> ExcluirImgColecao(string urlImagem)
        {
            // Extrai o nome do arquivo da URL do S3
            var uri = new Uri(urlImagem);
            var caminhoNoBucket = uri.AbsolutePath.TrimStart('/');

            try
            {
                var deleteRequest = new DeleteObjectRequest
                {
                    BucketName = BucketName,
                    Key = caminhoNoBucket
                };

                await _s3Client.DeleteObjectAsync(deleteRequest);
                return true;
            }
            catch (AmazonS3Exception ex)
            {
                // Log do erro se necessário
                Console.WriteLine($"Erro ao excluir imagem do S3: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteColecoesAsync(int id)
        {
            var colecao = await _context.Colecoes.FindAsync(id);
            if (colecao != null)
            {
                var delImg = await ExcluirImgColecao(colecao.ImagemColecao);
                if (!delImg)
                {
                    throw new Exception("Erro ao excluir a imagem da coleção.");
                }
                _context.Colecoes.Remove(colecao);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<ColecaoSimplesDto> EditarColecaoAsync(EditarColecaoDto colecao)
        {
            var exiteColecao = await _context.Colecoes.FindAsync(colecao.Id);
            if (exiteColecao != null)
            {
                if (colecao.Nome != null) { 
                    exiteColecao.Nome = colecao.Nome;
                    exiteColecao.Slug = GerarSlug(colecao.Nome);
                
                }
                if (colecao.DescColecao != null) exiteColecao.DescColecao = colecao.DescColecao;
                if (colecao.fotoColecao != null)
                {
                    var url = exiteColecao.ImagemColecao;
                    await ExcluirImgColecao(url);
                    var novaUrl = await SalvarImgColecao(colecao.fotoColecao);
                    exiteColecao.ImagemColecao = novaUrl;
                }
                _context.Colecoes.Update(exiteColecao);

                await _context.SaveChangesAsync();
                var resposta = new ColecaoSimplesDto
                {
                    Id = exiteColecao.Id,
                    Nome = exiteColecao.Nome,
                    DescColecao = exiteColecao.DescColecao,
                    ImagemColecao = exiteColecao.ImagemColecao
                };
                return resposta;
            }
            return null;

        }

        public async Task<ColecoesModel> PegarColecaoPorIdAsync(int id)
        {
            return await _context.Colecoes
            .Include(c => c.Produtos)
            .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<ColecoesModel?> PegarColecaoComProdutosEVariaçõesAsync(int id)
        {
            return await _context.Colecoes
       .Include(c => c.Produtos)
           .ThenInclude(p => p.Variacoes)
               .ThenInclude(v => v.Imagens)
       .FirstOrDefaultAsync(c => c.Id == id);
        }


        public async Task<IEnumerable<ColecoesModel>> PegarTodasColecoesAsync()
        {
            return await _context.Colecoes.Include(c => c.Produtos).ToListAsync();
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

        public async Task<ColecoesModel> PegarColecaoPorSlug(string slug)
        {
            var colecao = await _context.Colecoes
                .FirstOrDefaultAsync(c => c.Slug.Trim().ToLower() == slug.Trim().ToLower());

            if (colecao == null)
                throw new KeyNotFoundException($"Coleção com slug '{slug}' não encontrada.");

            return colecao;
        }

        


    }
}
