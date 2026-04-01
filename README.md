# Friday Modular Monolith

Code base `Friday` duoc scaffold theo huong:

- Single deployable (`Friday.API`)
- Modular architecture (`src/Modules/*`)
- Clean architecture (Domain -> Application -> Infrastructure -> API)
- DDD tactical patterns (Entity, AggregateRoot, ValueObject, DomainEvent)
- `BuildingBlocks` cho logic dung chung nhu event, unit of work, LinqToDB connection factory

## Current Modules

- `Sample`: module mau de tham khao cach to chuc module.

## Run

```bash
dotnet run --project src/API/Friday.API/Friday.API.csproj
```

Sample endpoints:

- `POST /api/sample/todos`
- `GET /api/sample/todos`

Admin endpoints:

- `POST /api/admin/users`
- `GET /api/admin/users`
- `POST /api/admin/users/{userId}/roles/{roleId}`
- `POST /api/admin/users/{userId}/lock`
- `POST /api/admin/roles`
- `GET /api/admin/roles`
- `POST /api/admin/roles/{roleId}/rights` (body: `int[]`)
- `POST /api/admin/rights`
- `GET /api/admin/rights`
