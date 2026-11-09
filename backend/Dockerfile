# Build and publish in the SDK image; the final container only needs the ASP.NET runtime.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

# Keep the repository SDK pin available during restore and compilation.
COPY global.json ./
COPY backend/ ./backend/

RUN dotnet restore backend/src/LogisticsEngine.Api/LogisticsEngine.Api.csproj
RUN dotnet publish backend/src/LogisticsEngine.Api/LogisticsEngine.Api.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

# Port 8080 is unprivileged; Compose publishes it on host port 5003 for local tools.
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# The named Compose volume is mounted here when JSON persistence is selected.
RUN mkdir -p /app/data && chown -R $APP_UID:$APP_UID /app
USER $APP_UID

ENTRYPOINT ["dotnet", "LogisticsEngine.Api.dll"]
