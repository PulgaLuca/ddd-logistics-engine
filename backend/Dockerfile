# Build multi-stage: l'SDK .NET serve solo a compilare, l'immagine finale
# contiene esclusivamente il runtime ASP.NET e i binari pubblicati.
# Il build context è la ROOT del repository (serve Directory.Build.props).

# ---------- Stage 1: restore + publish ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Proprietà MSBuild e regole di stile condivise da tutti i progetti.
COPY Directory.Build.props .editorconfig ./

# Prima solo i .csproj: se non cambiano, Docker riusa il layer di restore
# anche quando cambia il codice sorgente.
COPY backend/src/Dhexby.SharedKernel/Dhexby.SharedKernel.csproj \
     backend/src/Dhexby.SharedKernel/
COPY backend/src/Dhexby.LogisticsEngine.Domain/Dhexby.LogisticsEngine.Domain.csproj \
     backend/src/Dhexby.LogisticsEngine.Domain/
COPY backend/src/Dhexby.LogisticsEngine.Application/Dhexby.LogisticsEngine.Application.csproj \
     backend/src/Dhexby.LogisticsEngine.Application/
COPY backend/src/Dhexby.LogisticsEngine.Infrastructure/Dhexby.LogisticsEngine.Infrastructure.csproj \
     backend/src/Dhexby.LogisticsEngine.Infrastructure/
COPY backend/src/Dhexby.LogisticsEngine.Api/Dhexby.LogisticsEngine.Api.csproj \
     backend/src/Dhexby.LogisticsEngine.Api/

RUN dotnet restore backend/src/Dhexby.LogisticsEngine.Api/Dhexby.LogisticsEngine.Api.csproj

COPY backend/src backend/src

RUN dotnet publish backend/src/Dhexby.LogisticsEngine.Api/Dhexby.LogisticsEngine.Api.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish

# ---------- Stage 2: runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# curl serve solo all'healthcheck di Docker Compose.
RUN apt-get update \
    && apt-get install --yes --no-install-recommends curl \
    && rm --recursive --force /var/lib/apt/lists/*

# Kestrel in ascolto su HTTP: il TLS è responsabilità del reverse proxy.
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

COPY --from=build /app/publish .

# Utente non-root predefinito delle immagini .NET.
USER $APP_UID

ENTRYPOINT ["dotnet", "Dhexby.LogisticsEngine.Api.dll"]
