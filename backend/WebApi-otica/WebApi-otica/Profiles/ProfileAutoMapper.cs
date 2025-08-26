using AutoMapper;
using WebApi_otica.Dto.Colecoes;
using WebApi_otica.Dto.Pruduto.Criar;
using WebApi_otica.Dto.Pruduto.Editar;
using WebApi_otica.Dto.Pruduto.Vizualizar;
using WebApi_otica.Model;
using WebApi_otica.Model.Produto;

namespace WebApi_otica.Profiles
{
    public class ProfileAutoMapper : Profile
    {
        public ProfileAutoMapper()
        {
            // Mapeamento de CriarProdutoDto para ProdutosModel
            CreateMap<CriarProdutoDto, ProdutosModel>();

            // Mapeamento de CriarVariacaoDto para VariacaoModel
            CreateMap<CriarVariacaoDto, VariacaoModel>().ForMember(dest => dest.Imagens,opt => opt.Ignore());


            // Mapeamento de CriarImagemDto para ImagensProdModel
           // CreateMap<CriarImagemDto, ImagensProdModel>();

            // Mapeamento de EditarVariacaoDto para VariacaoModel
            CreateMap<EditarVariacaoDto, VariacaoModel>();

            // Mapeamento de EditarImagemDto para ImagensProdModel
            CreateMap<EditarImagemDto, ImagensProdModel>();

            // Mapeamento de ProdutosModel para VizualizarProdutoDto
            CreateMap<ProdutosModel, VizualizarProdutoDto>()
                .ForMember(dest => dest.NomeColecao, opt => opt.MapFrom(src => src.Colecao != null ? src.Colecao.Nome : null)) // Mapeando Colecao para NomeColecao
                .ForMember(dest => dest.Variacoes, opt => opt.MapFrom(src => src.Variacoes)); // Mapeando a lista de Variacoes

            // Mapeamento de VariacaoModel para VizualizarVariacaoDto
            CreateMap<VariacaoModel, VizualizarVariacaoDto>()
                .ForMember(dest => dest.Imagens, opt => opt.MapFrom(src => src.Imagens)); // Mapeando a lista de Imagens

            // Mapeamento de ImagensProdModel para VizualizarImagemDto
            CreateMap<ImagensProdModel, VizualizarImagemDto>();

            CreateMap<CriarColecaoDto, ColecoesModel>();

        }
    }
}
