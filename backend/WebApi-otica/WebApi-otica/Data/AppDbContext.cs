using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApi_otica.Model;
using WebApi_otica.Model.Produto;

namespace WebApi_otica.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ProdutosModel> Produtos { get; set; }
        public DbSet<ColecoesModel> Colecoes { get; set; }
        public DbSet<ImagensProdModel> ImagensProdutos { get; set; }
        public DbSet<VariacaoModel> VariacaoProdutos { get; set; }
        public DbSet<TagModel> Tags { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<VariacaoModel>()
             .HasMany(v => v.Tags)
             .WithMany(t => t.Variacoes)
             .UsingEntity(j => j.ToTable("VariacaoTags"));
        }



    }
}


