# MediaForge SaaS

[![CI](https://github.com/saturnnn7/MediaForgeAPI/actions/workflows/ci.yml/badge.svg)](https://github.com/saturnnn7/MediaForgeAPI/actions/workflows/ci.yml)

**Kyrylo Soprykin** - [LinkedIn](https://www.linkedin.com/in/kyrylo-soprykin/) | [GitHub](https://github.com/saturnnn7)

B2B SaaS platform for podcast and video-blogging. Users upload media files directly to object storage via pre-signed URLs. The system asynchronously transcodes to HLS, generates thumbnails, transcribes via OpenAI Whisper, indexes for full-text search, and notifies users in real time.

## Architecture

```
Client → Gateway.YARP → Identity.API / Media.API / Search.API / Catalog.API

Media.API → MinIO (pre-signed upload)
Media.API → RabbitMQ → Processing.Worker → FFmpeg → MinIO
Processing.Worker → MediaProcessingCompletedEvent → RabbitMQ
Gateway.YARP → SignalR → Client (real-time notification)
Search.API → Elasticsearch (indexes transcription + metadata)
Catalog.API → PostgreSQL (catalog DB)
Media.API ←→ Catalog.API (PartId cross-reference, no direct HTTP calls)
```

## Services

| Service | Port | Responsibility |
|---|---|---|
| Gateway.YARP | 5000 | Reverse proxy, rate limiting, SignalR hub |
| Identity.API | 5001 | OIDC provider (Duende IdentityServer), JWT RS256, gRPC server |
| Media.API | 5002 | Media asset management, pre-signed URL generation, gRPC client |
| Processing.Worker | - | FFmpeg transcoding, Whisper transcription, MinIO upload |
| Search.API | 5003 | Elasticsearch indexing and full-text search |
| Catalog.API | 5005 | Series, Works, Parts, Persons, Genres catalog |
| Notification (Gateway) | 5000 | SignalR hub embedded in Gateway |

## Tech Stack

| Category | Technology | Version |
|---|---|---|
| Runtime | .NET 8 LTS / C# 12 | 8.0.x |
| Auth | Duende IdentityServer | 7.0.x |
| Message Broker | MassTransit + RabbitMQ | 8.3.x |
| API Gateway | YARP | 2.3.x |
| Database | PostgreSQL + EF Core 8 | 16-alpine |
| Cache | Redis | 7.4 |
| Object Storage | MinIO (S3-compatible) | - |
| Search | Elasticsearch | 8.17.x |
| Media Processing | FFMpegCore + FFmpeg | 5.1.x |
| Transcription | OpenAI Whisper API | - |
| Real-time | ASP.NET Core SignalR | built-in |
| Inter-service | gRPC (Grpc.AspNetCore) | 2.67.x |
| Testing | xUnit + Testcontainers | 3.10.x |
| CI/CD | GitHub Actions | - |
| Containerization | Docker + Docker Compose | - |

## Key Design Decisions

- **Outbox Pattern** (MassTransit EF Core) - guarantees at-least-once delivery; no ghost records if broker is down during a request
- **Pre-signed URLs** - media files never transit the API servers; client uploads directly to MinIO
- **Pure Domain layer** - no ASP.NET Identity in Domain; IdentityAppUser lives in Infrastructure only
- **gRPC for synchronous inter-service calls** - Media.API validates user existence against Identity.API before persisting assets
- **FFmpeg concurrency semaphore** - limits parallel FFmpeg processes to 1; prevents CPU/RAM exhaustion on constrained hardware
- **Elasticsearch opt-in** - started via Docker Compose profile `--profile search`; InMemorySearchService used otherwise
- **Clean Architecture per service** - Domain → Application → Infrastructure → API; each service independently deployable

## Prerequisites

- .NET 8 SDK
- Docker Desktop (WSL2 backend on Windows)
- FFmpeg: `winget install ffmpeg`
- OpenSSL (comes with Git for Windows)

## Quick Start

### 1. Clone

```powershell
git clone https://github.com/saturnnn7/MediaForgeAPI.git
cd MediaForgeAPI
```

### 2. Generate RSA keys (first time only)

```powershell
mkdir src/Services/Identity/API/keys
openssl genrsa -out src/Services/Identity/API/keys/private.pem 2048
openssl rsa -in src/Services/Identity/API/keys/private.pem -pubout -out src/Services/Identity/API/keys/public.pem
cp src/Services/Identity/API/keys/public.pem src/Services/Media/API/keys/public.pem
```

### 3. Start infrastructure

```powershell
docker compose -f infra/docker-compose.yml up -d
```

### 4. Apply database migrations

```powershell
dotnet ef database update --project src/Services/Identity/Infrastructure --startup-project src/Services/Identity/API --context IdentityDbContext
dotnet ef database update --project src/Services/Identity/Infrastructure --startup-project src/Services/Identity/API --context ConfigurationDbContext
dotnet ef database update --project src/Services/Identity/Infrastructure --startup-project src/Services/Identity/API --context PersistedGrantDbContext
dotnet ef database update --project src/Services/Media/Infrastructure --startup-project src/Services/Media/API --context MediaDbContext
dotnet ef database update --project src/Services/Catalog/Infrastructure --startup-project src/Services/Catalog/API --context CatalogDbContext
```

### 5. Run a service

```powershell
dotnet run --project src/Services/Identity/API
```

## Running Tests

Single command:

```powershell
dotnet test MediaForge.sln
```

Testcontainers automatically starts all required infrastructure (PostgreSQL, RabbitMQ, MinIO, Elasticsearch) for each test project. No manual setup required.

Current coverage: 18 tests across 4 test projects.

## Project Structure

```
MediaForgeAPI/
├── src/
│   ├── Services/
│   │   ├── Identity/     # Domain / Application / Infrastructure / API
│   │   ├── Media/        # Domain / Application / Infrastructure / API
│   │   ├── Search/       # Domain / Application / Infrastructure / API
│   │   ├── Catalog/      # Domain / Application / Infrastructure / API
│   │   ├── Processing/   # Worker (BackgroundService)
│   │   └── Gateway/      # YARP + SignalR hub
│   └── Shared/
│       ├── Contracts/    # Integration events
│       └── Infrastructure/ # Base classes, Result<T>, Entity
├── tests/
│   ├── Identity.IntegrationTests/
│   ├── Media.IntegrationTests/
│   ├── Search.IntegrationTests/
│   └── Gateway.IntegrationTests/
├── infra/
│   ├── docker-compose.yml       # Infrastructure only
│   └── docker-compose.full.yml  # All services
└── bruno/                       # API collection (Bruno)
```
