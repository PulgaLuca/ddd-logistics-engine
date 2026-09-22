# LogisticsEngine

Template essenziale C# / .NET 10. La cartella `frontend/` è vuota.

## Struttura

```text
backend/
  LogisticsEngine.slnx
  Directory.Build.props
  Directory.Packages.props
  global.json
  src/
    LogisticsEngine.SharedKernel/
    LogisticsEngine.Domain/
    LogisticsEngine.Application/
    LogisticsEngine.Infrastructure/
    LogisticsEngine.WebApi/
  tests/
    LogisticsEngine.UnitTests/
    LogisticsEngine.IntegrationTests/
    LogisticsEngine.ArchitectureTests/
frontend/                           (vuota)
```

| Layer | Responsabilità | Riferimenti |
|---|---|---|
| SharedKernel | Tipi comuni Result ed Error | Nessun layer o pacchetto |
| Domain | Entità e regole di business da aggiungere | SharedKernel |
| Application | Casi d'uso, handler MediatR e validator FluentValidation | Domain |
| Infrastructure | Implementazioni delle interfacce applicative da aggiungere | Application |
| WebApi | ASP.NET Core, OpenAPI e composizione della dependency injection | Application, Infrastructure |

Application registra automaticamente handler e validator del proprio assembly.
La pipeline MediatR esegue la validazione; la Web API traduce gli errori FluentValidation in HTTP 400.
Infrastructure espone il metodo di registrazione delle future implementazioni.
Non sono inclusi moduli di business, database, cache, Docker o frontend.

## Dipendenze e test

Le versioni NuGet sono centralizzate in `backend/Directory.Packages.props`.

- Application: MediatR, FluentValidation e integrazione con dependency injection.
- WebApi: ASP.NET Core (SDK Web), Microsoft.AspNetCore.OpenApi e Microsoft.OpenApi.
- UnitTests: riferimenti a Domain/Application, xUnit, Shouldly e NSubstitute.
- IntegrationTests: riferimento a WebApi, Microsoft.AspNetCore.Mvc.Testing e WebApplicationFactory.
- ArchitectureTests: riferimento a WebApi (con i layer transitivi) e NetArchTest.Rules.
- Tutti i test: Microsoft.NET.Test.Sdk, xUnit runner e coverlet.collector.

I test di esempio verificano la pipeline di validazione, l'avvio HTTP e le dipendenze fra layer.

## Creare un nuovo progetto

Prerequisito: SDK .NET 10. Non servono Node.js o Docker.

Installa il template una volta, dalla sua cartella principale:

```powershell
dotnet new install .
```

Crea il nuovo progetto in una cartella distinta:

```powershell
dotnet new cleanarch -n Contoso.Billing -o ../Contoso.Billing
cd ../Contoso.Billing
New-Item -ItemType Directory -Force frontend
dotnet build backend/Contoso.Billing.slnx
dotnet test backend/Contoso.Billing.slnx --no-build
dotnet run --project backend/src/Contoso.Billing.WebApi
```

La generazione sostituisce `LogisticsEngine` nei nomi dei file, nei progetti, nei namespace e nei riferimenti.
Il ripristino NuGet parte automaticamente: scarica le dipendenze dichiarate nella cache locale.
Non si copiano `bin`, `obj` o librerie compilate. Serve accesso a NuGet al primo ripristino.
Se il ripristino automatico fallisce, esegui `dotnet restore backend/Contoso.Billing.slnx`.

Le directory vuote non vengono conservate da Git né copiate dal motore dei template: il comando
`New-Item` crea `frontend` completamente vuota anche nel nuovo progetto.

API locale: http://localhost:5000/health.
Documento OpenAPI in Development: http://localhost:5000/openapi/v1.json.

Per versionare la nuova copia in modo indipendente, esegui `git init` nella nuova cartella.
Non è necessario fare sostituzioni manuali o clonare la cronologia del template.

Documentazione del motore: [template personalizzati .NET](https://learn.microsoft.com/en-us/dotnet/core/tools/custom-templates).
