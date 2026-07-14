FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
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
COPY ["src/Services/Search/Domain/MediaForge.Search.Domain.csproj", "src/Services/Search/Domain/"]
COPY ["src/Services/Search/Application/MediaForge.Search.Application.csproj", "src/Services/Search/Application/"]
COPY ["src/Services/Search/Infrastructure/MediaForge.Search.Infrastructure.csproj", "src/Services/Search/Infrastructure/"]
COPY ["src/Services/Search/API/MediaForge.Search.API.csproj", "src/Services/Search/API/"]

RUN dotnet restore "src/Services/Search/API/MediaForge.Search.API.csproj"

# Copy all source
COPY . .

RUN dotnet publish "src/Services/Search/API/MediaForge.Search.API.csproj" \
    -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "MediaForge.Search.API.dll"]
