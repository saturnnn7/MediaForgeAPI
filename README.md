# MediaForge SaaS

> Real-Time Media Processing & Analytics B2B SaaS Platform

## Architecture

```
[Client] → [Gateway:5000] → [Identity.API:5001]
                           → [Media.API:5002]
                           → [Search.API:5003]
                           ↕ SignalR /hubs/notifications

[Media.API] → [MinIO] (pre-signed upload)
            → [RabbitMQ] → [Processing.Worker]
                              → FFmpeg (transcode/HLS)
                              → OpenAI Whisper (transcription)
                              → [MinIO] (outputs)
                              → [Search.API] (index transcription)
                              → [Gateway] → SignalR → [Client]
```

## Tech Stack

| Service / Concern | Technology | Purpose |
|---|---|---|
| Runtime | .NET 8 LTS / C# 12 | All services |
| Identity | Duende IdentityServer 7.0.6 | OIDC provider, token issuance |
| Messaging | MassTransit 8.3.6 + RabbitMQ | Async inter-service communication |
| Messaging | MassTransit StateMachine | Long-running workflow orchestration |
| Reliability | MassTransit EF Core Outbox | Exactly-once publish on every publishing service |
| Persistence | EF Core 8 + Npgsql | Identity.API, Media.API (PostgreSQL 16-alpine) |
| Inter-service RPC | gRPC | Media.API → Identity.API (user data lookups) |
| Object storage | MinIO (AWSSDK.S3) | Pre-signed upload/download URLs |
| Search | Elasticsearch 8.17.x (Elastic.Clients.Elasticsearch 8.17.4) | Transcription/media indexing |
| Cache | Redis 7.4 (StackExchange.Redis) | Distributed cache, SignalR backplane |
| Gateway | YARP 2.3 | Reverse proxy + SignalR hub |
| Media processing | FFMpegCore | Transcode / HLS packaging |
| Transcription | OpenAI Whisper | Audio-to-text |
| Testing | Testcontainers | Integration tests against real infra |

## Services

- **Identity.API** - Duende IdentityServer OIDC provider: registration, login, token issuance/refresh, and a gRPC endpoint exposing user data to other services.
- **Media.API** - Owns media asset metadata; issues pre-signed MinIO upload URLs, confirms uploads, and publishes integration events for processing.
- **Processing.Worker** - Background service consuming media-uploaded events; runs FFmpeg transcode/HLS packaging and Whisper transcription, then re-uploads outputs to MinIO.
- **Search.API** - Indexes transcriptions/media metadata into Elasticsearch and serves search queries.
- **Gateway.YARP** - Single ingress point: YARP reverse proxy routing to the three APIs, plus a SignalR hub for pushing processing-status notifications to clients.
- **Shared.Contracts** - Integration event contracts (`IIntegrationEvent`) shared across services via RabbitMQ.
- **Shared.Infrastructure** - Common EF base types, `Result<T>`, guard clauses.

## Prerequisites

- .NET 8 SDK
- Docker Desktop with WSL2
- `openssl` (for RSA key generation used by Identity.API's JWT signing)

## Quick Start (development)

### 1. Start infrastructure

```powershell
docker compose -f infra/docker-compose.yml up -d
```

### 2. Generate RSA keys (first time only)

Identity.API signs access tokens with RS256 using a PEM key pair at `src/Services/Identity/API/keys/`:

```powershell
openssl genrsa -out src/Services/Identity/API/keys/private.pem 2048
openssl rsa -in src/Services/Identity/API/keys/private.pem -pubout -out src/Services/Identity/API/keys/public.pem
```

### 3. Apply database migrations

```powershell
dotnet ef migrations add InitialCreate --project src/Services/Identity/Infrastructure --startup-project src/Services/Identity/API
dotnet ef database update --project src/Services/Identity/Infrastructure --startup-project src/Services/Identity/API

dotnet ef migrations add InitialCreate --project src/Services/Media/Infrastructure --startup-project src/Services/Media/API
dotnet ef database update --project src/Services/Media/Infrastructure --startup-project src/Services/Media/API
```

Search.API has no EF migrations - it indexes into Elasticsearch, not PostgreSQL.

### 4. Run a service

```powershell
cd src/Services/Identity/API && dotnet run
```

### Full stack (Docker)

```powershell
docker compose -f infra/docker-compose.full.yml up -d
```

This builds and runs every .NET service alongside infrastructure - intended for demos/CI, not day-to-day development (no hot reload, full rebuild on every code change).

## Running Tests

```powershell
dotnet test --verbosity normal
```

## Key Design Decisions

- Monorepo with Central Package Management (`Directory.Packages.props`)
- Clean Architecture per service (Domain → Application → Infrastructure → API)
- MassTransit Outbox Pattern (no ghost records on publish failure)
- Pure Domain - no ASP.NET Identity types leak into the Domain layer
- Pre-signed URLs - media files never transit the API servers
- FFmpeg concurrency limited to 1 (laptop-friendly for local dev)
- Elasticsearch is opt-in via the `search` docker compose profile
