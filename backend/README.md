# Dash API

## Database Setup

This project uses Entity Framework Core with a SQLite provider for development.

### 1. Prerequisites

Ensure the EF Core CLI tools are installed:

```bash
dotnet tool install --global dotnet-ef
```

### 2. Apply Migrations

To generate the local dev.db SqLite file and create the schema, run this command from the root of the repository:

```bash
dotnet ef database update \
  --project src/Dash.Infrastructure/Dash.Infrastructure.csproj \
  --startup-project src/Dash.Api/Dash.Api.csproj
```

### 3. Creating new Migrations

If you modify models in Dash.Domain, generate a new migration with:

```bash
dotnet ef migrations add <MigrationName> \
  --project src/Dash.Infrastructure/Dash.Infrastructure.csproj \
  --startup-project src/Dash.Api/Dash.Api.csproj
```

---
