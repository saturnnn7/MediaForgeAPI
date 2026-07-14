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
COPY ["src/Services/Media/Domain/MediaForge.Media.Domain.csproj", "src/Services/Media/Domain/"]
COPY ["src/Services/Media/Application/MediaForge.Media.Application.csproj", "src/Services/Media/Application/"]
COPY ["src/Services/Media/Infrastructure/MediaForge.Media.Infrastructure.csproj", "src/Services/Media/Infrastructure/"]
COPY ["src/Services/Media/API/MediaForge.Media.API.csproj", "src/Services/Media/API/"]

RUN dotnet restore "src/Services/Media/API/MediaForge.Media.API.csproj"

# Copy all source
COPY . .

RUN dotnet publish "src/Services/Media/API/MediaForge.Media.API.csproj" \
    -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "MediaForge.Media.API.dll"]
