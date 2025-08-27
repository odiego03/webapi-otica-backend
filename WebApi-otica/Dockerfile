# Etapa de build e publish
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["WebApi-otica/WebApi-otica.csproj", "WebApi-otica/"]
RUN dotnet restore "WebApi-otica/WebApi-otica.csproj"

COPY . .
WORKDIR "/src/WebApi-otica"
RUN dotnet publish "WebApi-otica.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WebApi-otica.dll"]
