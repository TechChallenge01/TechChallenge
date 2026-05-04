FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["API.Test/API.Test.csproj", "API.Test/"]
COPY ["API/API.csproj", "API/"]

RUN dotnet restore "API.Test/API.Test.csproj"

COPY . .

RUN dotnet build "API.Test/API.Test.csproj" -c Release --no-restore

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS test
WORKDIR /src

COPY --from=build /src .

ENTRYPOINT ["dotnet", "test", "API.Test/API.Test.csproj", \
            "--no-build", \
            "-c", "Release", \
            "--logger", "trx;LogFileName=test-results.trx", \
            "--results-directory", "/test-results"]