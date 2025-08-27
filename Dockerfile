# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia tudo para o container
COPY . .

# Restaura dependências e publica o projeto
RUN dotnet restore WebApi-otica/WebApi-otica.csproj
RUN dotnet publish WebApi-otica/WebApi-otica.csproj -c Release -o /app/publish

# Etapa de runtime (imagem mais leve)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copia o resultado da publicação
COPY --from=build /app/publish .

# Define o entrypoint (roda a API)
ENTRYPOINT ["dotnet", "WebApi-otica.dll"]
