FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app

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
COPY ["src/Services/Processing/Worker/MediaForge.Processing.Worker.csproj", "src/Services/Processing/Worker/"]

RUN dotnet restore "src/Services/Processing/Worker/MediaForge.Processing.Worker.csproj"

# Copy all source
COPY . .

RUN dotnet publish "src/Services/Processing/Worker/MediaForge.Processing.Worker.csproj" \
    -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
RUN apk add --no-cache ffmpeg
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "MediaForge.Processing.Worker.dll"]
