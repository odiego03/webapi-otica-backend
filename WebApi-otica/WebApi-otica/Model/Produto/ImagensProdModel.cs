using System.Text.Json.Serialization;
using WebApi_otica.Model.Produto;

public class ImagensProdModel
{
    public int Id { get; set; }
    public int VariacaoId { get; set; }
    public VariacaoModel Variacao { get; set; }

    public string UrlImagem { get; set; }
}
