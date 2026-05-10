# ASP.NET Host Prototype

This folder is intentionally separate from the working WinUI application. It is a prototype SaaS-style backend that exposes HTTP endpoints over the same SQL databases.

## Projects

- `Matchmaking.Api/` - ASP.NET Core Web API.
- `Matchmaking.Host.sln` - separate solution file for the host prototype.

## Local Run

From the repository root:

```powershell
dotnet run --project "aspnet-host\Matchmaking.Api\Matchmaking.Api.csproj"
```

Default development connection strings are in `Matchmaking.Api/appsettings.Development.json`.

For a team/shared deployment, prefer environment variables or user secrets:

```powershell
dotnet user-secrets init --project "aspnet-host\Matchmaking.Api\Matchmaking.Api.csproj"
dotnet user-secrets set "ConnectionStrings:CommunityConnection" "Server=YOUR_SERVER;Database=Communities;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True" --project "aspnet-host\Matchmaking.Api\Matchmaking.Api.csproj"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=YOUR_SERVER;Database=matchmaking_db;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True" --project "aspnet-host\Matchmaking.Api\Matchmaking.Api.csproj"
```

## Current Endpoints

- `GET /api/health`
- `POST /api/auth/login`
- `GET /api/communities/users/{userId}`
- `GET /api/tinder/profile/{userId}`
- `GET /api/tinder/discover/{userId}`

Example login body:

```json
{
  "email": "alex@boards.wp",
  "password": "password"
}
```

## Deployment Shape

Deploy the API to Azure App Service, a university server, or another ASP.NET host. Deploy the databases to Azure SQL or another reachable SQL Server. The WinUI app should eventually call this API instead of connecting directly to SQL.

## Important

This prototype does not change the current working WinUI app. It is a safe separate starting point for moving toward a SaaS backend.
