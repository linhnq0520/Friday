# Blazor WASM on Azure Static Web Apps with GitHub Actions

Personal sites are a perfect free-tier lab. Blazor WebAssembly compiles to static assets, so **Azure Static Web Apps Free** fits better than a always-on App Service.

## Project layout in this repo

```text
src/UI/Friday.Portfolio/   # Blazor WASM app (same UI folder convention as AdminPortal)
docs/portfolio-azure-deploy-guide.md
.github/workflows/azure-static-web-apps-portfolio.yml
```

## Pipeline shape

1. `actions/setup-dotnet` → .NET 10
2. `dotnet publish` the portfolio project
3. `Azure/static-web-apps-deploy` with `skip_app_build: true` pointing at `wwwroot`

## Secrets

Store only the deployment token in GitHub Actions:

`AZURE_STATIC_WEB_APPS_API_TOKEN_PORTFOLIO`

Never commit tokens. Rotate from the Azure portal if leaked.

## Why this is good for learning

- Real CI/CD without paying for a server
- PR staging environments on Static Web Apps
- Same monorepo discipline as the Friday API (`src/`, central `Directory.Packages.props`)
