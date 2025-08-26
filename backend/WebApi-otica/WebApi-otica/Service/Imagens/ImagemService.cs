using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using WebApi_otica.Data;
using WebApi_otica.Dto.Pruduto.Vizualizar;
using Amazon.S3;
using Amazon.S3.Model;

namespace WebApi_otica.Service.Imagens
{
    public class ImagemService : IImagemService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAmazonS3 _s3Client; // Cliente S3
        private const string BucketName = "hk-ecommerce-imagens"; // Nome do seu bucket

        public ImagemService(AppDbContext context, IMapper mapper, IAmazonS3 s3Client) // Adicione aqui
        {
            _context = context;
            _mapper = mapper;
            _s3Client = s3Client; // Injete o cliente
        }


        public async Task ExcluirImagemFisicaAsync(string urlImagem)
        {
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
            }
            catch (AmazonS3Exception ex)
            {
                // Log do erro se necessário
                Console.WriteLine($"Erro ao excluir imagem do S3: {ex.Message}");
            }
        }


        public async Task<ImagensProdModel> SalvarImagemAsync(IFormFile imagem)
        {
            var nomeArquivo = Guid.NewGuid() + Path.GetExtension(imagem.FileName);
            var caminhoNoBucket = $"produtos/{nomeArquivo}";

            try
            {
                var request = new PutObjectRequest
                {
                    BucketName = BucketName,
                    Key = caminhoNoBucket,
                    InputStream = imagem.OpenReadStream(),
                    ContentType = imagem.ContentType
                };

                await _s3Client.PutObjectAsync(request);

                // Gera a URL pública da imagem no S3
                var urlPublica = $"https://{BucketName}.s3.sa-east-1.amazonaws.com/{caminhoNoBucket}";

                return new ImagensProdModel
                {
                    UrlImagem = urlPublica // Agora retorna a URL completa do S3
                };
            }
            catch (Exception ex)
            {
                // Log do erro (você pode melhorar isso)
                throw new Exception($"Erro ao salvar imagem no S3: {ex.Message}");
            }
        }


        public async Task<bool> ExcluirImagemAsync(int imagemId)
        {
            var imagens = await _context.ImagensProdutos.FindAsync(imagemId);
            if (imagens == null) return false;
            await ExcluirImagemFisicaAsync(imagens.UrlImagem);
            _context.ImagensProdutos.Remove(imagens);
            await _context.SaveChangesAsync();
            return true;

        }

        public async Task<List<VizualizarImagemDto>> AddImagemAsync(int variacaoId, List<IFormFile> arquivos)
        {
            var variacao = await _context.VariacaoProdutos.FindAsync(variacaoId);
            if (variacao == null) throw new ArgumentException("Variação não encontrada.");



            var imagens = new List<ImagensProdModel>();
            var tiposPermitidos = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var tamanhoMaximo = 5 * 1024 * 1024;// 5 MB

            foreach (var arquivo in arquivos)
            {
                var extensao = Path.GetExtension(arquivo.FileName).ToLowerInvariant();

                if (!tiposPermitidos.Contains(extensao))
                    throw new ArgumentException($"Tipo de arquivo não permitido: {extensao}");

                if (arquivo.Length > tamanhoMaximo)
                    throw new ArgumentException("O arquivo excede o tamanho máximo permitido de 5MB.");

                var url = await SalvarImagemAsync(arquivo);
                var imagem = new ImagensProdModel
                {
                    VariacaoId = variacaoId,
                    UrlImagem = url.UrlImagem
                };
                _context.ImagensProdutos.Add(imagem);
                imagens.Add(imagem);
            }
            await _context.SaveChangesAsync();

            var resultado = _mapper.Map<List<VizualizarImagemDto>>(imagens);
            return resultado;


        }
    }
}
