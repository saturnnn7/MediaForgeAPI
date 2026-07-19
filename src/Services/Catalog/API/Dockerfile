FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and all csproj files first (layer cache for restore)
COPY ["MediaForge.sln", "."]
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY [".editorconfig", "."]
COPY ["global.json", "."]

# Copy csproj files for this service's dependency graph
COPY ["src/Shared/Contracts/MediaForge.Shared.Contracts.csproj", "src/Shared/Contracts/"]
COPY ["src/Shared/Infrastructure/MediaForge.Shared.Infrastructure.csproj", "src/Shared/Infrastructure/"]
COPY ["src/Services/Catalog/Domain/MediaForge.Catalog.Domain.csproj", "src/Services/Catalog/Domain/"]
COPY ["src/Services/Catalog/Application/MediaForge.Catalog.Application.csproj", "src/Services/Catalog/Application/"]
COPY ["src/Services/Catalog/Infrastructure/MediaForge.Catalog.Infrastructure.csproj", "src/Services/Catalog/Infrastructure/"]
COPY ["src/Services/Catalog/API/MediaForge.Catalog.API.csproj", "src/Services/Catalog/API/"]

RUN dotnet restore "src/Services/Catalog/API/MediaForge.Catalog.API.csproj"

# Copy all source
COPY . .

RUN dotnet publish "src/Services/Catalog/API/MediaForge.Catalog.API.csproj" \
    -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "MediaForge.Catalog.API.dll"]
