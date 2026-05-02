# ─────────────────────────────────────────────
# Dockerfile para os Testes de Integração
# Constrói e executa o projeto Integration.test
# ─────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS test-runner
WORKDIR /src

# Copia csproj de todos os projetos
COPY src/API/API.csproj                                          src/API/
COPY src/Application/Application.csproj                          src/Application/
COPY src/Domain/Domain.csproj                                    src/Domain/
COPY src/Infra/Infra.csproj                                      src/Infra/
COPY src/Shared/Shared.csproj                                    src/Shared/
COPY tests/TestesIntegracao/Integration.test/Integration.test.csproj \
     tests/TestesIntegracao/Integration.test/

# Restaura incluindo o projeto de testes
RUN dotnet restore tests/TestesIntegracao/Integration.test/Integration.test.csproj

# Copia todo o código-fonte e os testes
COPY src/   src/
COPY tests/ tests/

# Comando padrão: roda os testes de integração com output detalhado
ENTRYPOINT ["dotnet", "test", \
    "tests/TestesIntegracao/Integration.test/Integration.test.csproj", \
    "--no-restore", \
    "--logger", "console;verbosity=detailed", \
    "--results-directory", "/test-results"]