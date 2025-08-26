# Imagem base para runtime (produção)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Imagem para build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["WebApi-otica/WebApi-otica.csproj", "WebApi-otica/"]
RUN dotnet restore "WebApi-otica/WebApi-otica.csproj"

COPY . .
WORKDIR "/src/WebApi-otica"
RUN dotnet build "WebApi-otica.csproj" -c Release -o /app/build

# Publicação
FROM build AS publish
RUN dotnet publish "WebApi-otica.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApi-otica.dll"]
