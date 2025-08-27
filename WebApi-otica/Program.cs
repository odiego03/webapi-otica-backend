using Amazon.S3;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using WebApi_otica.Data;
using WebApi_otica.Service;
using WebApi_otica.Service.Autenticacao;
using WebApi_otica.Service.Imagens;
using WebApi_otica.Service.Produto;
using WebApi_otica.Service.Tags;
using WebApi_otica.Service.Variacao;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173", "http://localhost:3001", "http://localhost:5170") // <- origem do front
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});
builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    // Obtenha as chaves do appsettings.json
    var accessKey = builder.Configuration["AWS:AccessKeyId"];
    var secretKey = builder.Configuration["AWS:SecretAccessKey"];

    var config = new AmazonS3Config
    {
        RegionEndpoint = Amazon.RegionEndpoint.USEast2
    };

    // Passe as credenciais para o cliente
    return new AmazonS3Client(accessKey, secretKey, config);
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT authorization header using the Bearer scheme." +
        " \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below." +
        " \r\n\r\r Exemple: \"Bearer 12345abcdef\" ",
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:key"]))
        };
    });


builder.Services.AddScoped<IAutenticacao, AuthenticateService>();
builder.Services.AddScoped<IProdutoService, ProdutoService>();
builder.Services.AddScoped<IImagemService, ImagemService>();
builder.Services.AddScoped<IVariacaoService, VariacaoService>();
builder.Services.AddScoped<IColecoesService, ColecoesService>();
builder.Services.AddScoped<ITagService, TagService>();

builder.Services.AddAutoMapper(typeof(WebApi_otica.Profiles.ProfileAutoMapper));

var app = builder.Build();


    app.UseSwagger();
    app.UseSwaggerUI();




app.UseRouting();
//app.UseHttpsRedirection();
app.UseCors("PermitirFrontend");
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
