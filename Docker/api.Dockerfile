FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copia apenas os csproj primeiro para aproveitar cache de camadas do Docker
COPY src/API/API.csproj                        src/API/
COPY src/Application/Application.csproj        src/Application/
COPY src/Domain/Domain.csproj                  src/Domain/
COPY src/Infra/Infra.csproj                    src/Infra/
COPY src/Shared/Shared.csproj                  src/Shared/

# Restaura dependências
RUN dotnet restore

# Copia todo o restante do código-fonte
COPY src/ src/

# Publica em modo Release
RUN dotnet publish src/API/API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Variáveis de ambiente padrão — sobrescreva no docker-compose
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:8080

# Copia apenas o artefato publicado
COPY --from=build /app/publish .

# Porta exposta (documentação; o mapeamento real fica no compose)
EXPOSE 8080

ENTRYPOINT ["dotnet", "API.dll"]