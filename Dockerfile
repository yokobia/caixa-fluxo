# STAGE 1: Base de Runtime (Imagem otimizada e leve para execução)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# STAGE 2: SDK de Compilação (Utiliza o SDK do .NET para buildar a árvore de projetos)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copia os arquivos de projeto (.csproj) para restaurar os pacotes em cache (Otimiza o build)
COPY ["CaixaFluxo.Domain/CaixaFluxo.Domain.csproj", "CaixaFluxo.Domain/"]
COPY ["CaixaFluxo.Application/CaixaFluxo.Application.csproj", "CaixaFluxo.Application/"]
COPY ["CaixaFluxo.Infrastructure/CaixaFluxo.Infrastructure.csproj", "CaixaFluxo.Infrastructure/"]
COPY ["CaixaFluxo.API/CaixaFluxo.API.csproj", "CaixaFluxo.API/"]
COPY ["CaixaFluxo.Tests/CaixaFluxo.Tests.csproj", "CaixaFluxo.Tests/"]

# Executa o restore global de todas as dependências mapeadas
RUN dotnet restore "CaixaFluxo.API/CaixaFluxo.API.csproj"

# Copia todo o restante do código fonte do computador para dentro do container
COPY . .
WORKDIR "/src/CaixaFluxo.API"

# Compila o projeto em modo de otimização de produção
RUN dotnet build "CaixaFluxo.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# STAGE 3: Publicação dos artefatos finais compilados
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CaixaFluxo.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# STAGE 4: Imagem Final (Une os artefatos publicados ao ambiente leve de runtime do ASP.NET)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CaixaFluxo.API.dll"]
