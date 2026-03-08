# dotnet-microservices-demo

A small .NET 10 microservices portfolio project in C# designed for recruiter review.

## Why this repo exists

This demo shows practical backend engineering fundamentals in a clean, easy-to-review format:

- Minimal API microservices in .NET 10
- Service boundaries (`Catalog`, `Orders`, `Gateway`)
- Shared contracts and deterministic pricing logic
- Unit testing with xUnit
- Containerized local orchestration with Docker Compose

## Architecture

- `Catalog.Api` (`src/Services/Catalog/Catalog.Api`)
- Owns product catalog data and read endpoints.
- `Orders.Api` (`src/Services/Orders/Orders.Api`)
- Accepts checkout requests and calculates order totals.
- `Gateway.Api` (`src/Gateway/Gateway.Api`)
- Single entry point that aggregates/forwards shop requests.
- `Shared.Contracts` (`src/BuildingBlocks/Shared.Contracts`)
- DTOs and pricing engine used by services.
- `Services.Tests` (`tests/Services.Tests`)
- Unit tests for core pricing behavior.

## Quick start (local)

Prerequisite: .NET 10 SDK

```powershell
dotnet --version
dotnet restore
dotnet test
dotnet run --project src/Services/Catalog/Catalog.Api --urls http://localhost:5001
dotnet run --project src/Services/Orders/Orders.Api --urls http://localhost:5002
dotnet run --project src/Gateway/Gateway.Api --urls http://localhost:5000
```

After all 3 apps are running:

- Catalog via gateway: `GET http://localhost:5000/api/shop/catalog`
- Create order: `POST http://localhost:5000/api/shop/checkout`
- List orders: `GET http://localhost:5000/api/shop/orders`

Sample checkout request body:

```json
{
  "customerEmail": "candidate@example.com",
  "lines": [
    { "catalogItemId": 1, "quantity": 2 },
    { "catalogItemId": 2, "quantity": 1 }
  ]
}
```

## Quick start (Docker)

Prerequisite: Docker Desktop

```powershell
docker compose up --build
```

Gateway is available at `http://localhost:5000`.

## API endpoints

### Catalog

- `GET /health`
- `GET /api/catalog/items`
- `GET /api/catalog/items/{id}`

### Orders

- `GET /health`
- `GET /api/orders`
- `GET /api/orders/{id}`
- `POST /api/orders`

### Gateway

- `GET /health`
- `GET /api/shop/catalog`
- `GET /api/shop/orders`
- `POST /api/shop/checkout`

## Notes for reviewers

- This project intentionally keeps data in memory to focus on microservice boundaries and API flow.
- Next production steps would include persistence, distributed tracing, auth, and async messaging.
"# dotnet-microservices-demo" 
