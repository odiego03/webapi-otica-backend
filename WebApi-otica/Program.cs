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

// LOGS DE DEBUG CRÍTICOS
Console.WriteLine("🚀 Iniciando aplicação no Railway...");
Console.WriteLine($"PORT: {Environment.GetEnvironmentVariable("PORT")}");
Console.WriteLine($"ASPNETCORE_ENVIRONMENT: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}");

// Configuração da porta do Railway
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://*:{port}");

builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
});

builder.Services.AddHealthChecks();
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173", "http://localhost:3001", "http://localhost:5170")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// AWS S3 com fallback seguro
builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    try
    {
        var accessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
        var secretKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");
        var region = Environment.GetEnvironmentVariable("AWS_REGION") ?? "us-east-2";

        Console.WriteLine($"🔍 AWS AccessKey presente: {!string.IsNullOrEmpty(accessKey)}");
        Console.WriteLine($"🔍 AWS SecretKey presente: {!string.IsNullOrEmpty(secretKey)}");

        if (string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(secretKey))
        {
            Console.WriteLine("⚠️ AVISO: Variáveis AWS não configuradas - usando modo fallback");
            return null; // ou criar um client mock
        }

        var config = new AmazonS3Config
        {
            RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(region)
        };

        return new AmazonS3Client(accessKey, secretKey, config);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Erro ao configurar S3: {ex.Message}");
        return null;
    }
});

// Swagger
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
        Description = "JWT authorization header using the Bearer scheme.",
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


// Database - Configuração segura
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");

if (string.IsNullOrEmpty(connectionString))
{
    // Fallback para desenvolvimento local
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    Console.WriteLine("⚠️ AVISO: DATABASE_URL não encontrada, usando fallback local");
}

Console.WriteLine($"🔍 Connection String presente: {!string.IsNullOrEmpty(connectionString)}");

if (!string.IsNullOrEmpty(connectionString))
{
    try
    {
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });
        Console.WriteLine("✅ Database configurado com sucesso");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Erro ao configurar database: {ex.Message}");
        // Não crasha a aplicação - permite health checks
    }
}
else
{
    Console.WriteLine("⚠️ AVISO: Nenhuma connection string disponível");
    // A aplicação sobe mesmo sem DB para health checks
}

// Identity e JWT
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? builder.Configuration["Jwt:Key"];
Console.WriteLine($"🔍 JWT Key presente: {!string.IsNullOrEmpty(jwtKey)}");

if (!string.IsNullOrEmpty(jwtKey))
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? builder.Configuration["Jwt:Issuer"],
                ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
            };
        });
}

// Services
builder.Services.AddScoped<IAutenticacao, AuthenticateService>();
builder.Services.AddScoped<IProdutoService, ProdutoService>();
builder.Services.AddScoped<IImagemService, ImagemService>();
builder.Services.AddScoped<IVariacaoService, VariacaoService>();
builder.Services.AddScoped<IColecoesService, ColecoesService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddAutoMapper(typeof(WebApi_otica.Profiles.ProfileAutoMapper));

var app = builder.Build();

// Middleware pipeline
Console.WriteLine("✅ Build completo - Configurando middleware...");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/health");
app.UseRouting();
app.UseCors("PermitirFrontend");
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
Console.WriteLine("🔄 Verificando e aplicando migrations...");
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<AppDbContext>();

        // Verifica se existem migrations pendentes
        var pendingMigrations = dbContext.Database.GetPendingMigrations();
        if (pendingMigrations.Any())
        {
            Console.WriteLine($"📋 Migrations pendentes: {string.Join(", ", pendingMigrations)}");
            dbContext.Database.Migrate();
            Console.WriteLine("✅ Todas as migrations foram aplicadas!");
        }
        else
        {
            Console.WriteLine("✅ Nenhuma migration pendente");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Erro ao processar migrations: {ex.Message}");
        // Não crasha a app - apenas registra o erro
    }
}

Console.WriteLine($"✅ Aplicação iniciada na porta: {port}");
Console.WriteLine($"🌐 Ambiente: {app.Environment.EnvironmentName}");
Console.WriteLine($"✅ Aplicação iniciada na porta: {port}");
Console.WriteLine($"🌐 Ambiente: {app.Environment.EnvironmentName}");

app.Run();