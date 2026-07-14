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
COPY ["src/Services/Identity/Domain/MediaForge.Identity.Domain.csproj", "src/Services/Identity/Domain/"]
COPY ["src/Services/Identity/Application/MediaForge.Identity.Application.csproj", "src/Services/Identity/Application/"]
COPY ["src/Services/Identity/Infrastructure/MediaForge.Identity.Infrastructure.csproj", "src/Services/Identity/Infrastructure/"]
COPY ["src/Services/Identity/API/MediaForge.Identity.API.csproj", "src/Services/Identity/API/"]

RUN dotnet restore "src/Services/Identity/API/MediaForge.Identity.API.csproj"

# Copy all source
COPY . .

RUN dotnet publish "src/Services/Identity/API/MediaForge.Identity.API.csproj" \
    -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "MediaForge.Identity.API.dll"]
