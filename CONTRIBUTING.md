# Contributing

Thanks for helping improve the Personal Site API.

## Workflow

1. Fork the repository (or work on a branch if you have write access).
2. Create a focused branch: `feat/...`, `fix/...`, or `chore/...`.
3. Keep changes aligned with Clean Architecture:
   - Domain stays free of ASP.NET and EF Core types.
   - Application holds use cases, validation, and MediatR handlers.
   - Infrastructure owns persistence.
   - Api owns HTTP, CORS, and OpenAPI.
4. Open a pull request against `main`.

## Local checks

```bash
docker compose up -d
dotnet restore
dotnet build
```

The companion UI is [front_personalsite](https://github.com/seyedWebpro/front_personalsite). If your change affects the public contract, note it in the PR.

## Commit style

Use a short imperative subject that says **why** the change exists, e.g. `fix: reject empty profile payloads`.
