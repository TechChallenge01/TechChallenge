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

# Gera o EF Core migration bundle (executável self-contained, independente de dotnet
# runtime) — é isso que o k8s/migration-job.yaml executa como /app/efbundle.
# Ajuste --project/--startup-project se o DbContext estiver em outro projeto que não o Infra.
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"
RUN dotnet ef migrations bundle \
    --project src/Infra/Infra.csproj \
    --startup-project src/API/API.csproj \
    --configuration Release \
    --self-contained \
    -r linux-x64 \
    -o /app/efbundle


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Variáveis de ambiente padrão — sobrescrita pelo ConfigMap no Kubernetes
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:8080

# Copia o artefato publicado e o migration bundle
COPY --from=build /app/publish .
COPY --from=build /app/efbundle .

# Porta exposta
EXPOSE 8080

ENTRYPOINT ["dotnet", "API.dll"]