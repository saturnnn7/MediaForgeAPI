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
COPY ["src/Services/Gateway/MediaForge.Gateway.YARP.csproj", "src/Services/Gateway/"]

RUN dotnet restore "src/Services/Gateway/MediaForge.Gateway.YARP.csproj"

# Copy all source
COPY . .

RUN dotnet publish "src/Services/Gateway/MediaForge.Gateway.YARP.csproj" \
    -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "MediaForge.Gateway.YARP.dll"]
