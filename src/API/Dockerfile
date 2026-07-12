FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copia a solution (melhora o restore em projetos multi-camada)
COPY *.sln .

# Copia apenas os csproj primeiro (cache de dependência)
COPY src/API/API.csproj src/API/
COPY src/Application/Application.csproj src/Application/
COPY src/Domain/Domain.csproj src/Domain/
COPY src/Infra/Infra.csproj src/Infra/
COPY src/Shared/Shared.csproj src/Shared/

# Restaura dependências (apontando pro projeto principal)
RUN dotnet restore src/API/API.csproj

# Copia todo o restante do código
COPY src/ src/

# Publica em modo Release
RUN dotnet publish src/API/API.csproj \
    -c Release \
    -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Variáveis de ambiente padrão — sobrescrita pelo ConfigMap no Kubernetes
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:8080

# Copia apenas o artefato publicado
COPY --from=build /app/publish .

# Porta exposta
EXPOSE 8080

ENTRYPOINT ["dotnet", "API.dll"]
