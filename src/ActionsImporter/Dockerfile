FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY src/Directory.Build.props src/
COPY src/ActionsImporter/ActionsImporter.csproj src/ActionsImporter/
RUN dotnet restore src/ActionsImporter/ActionsImporter.csproj
COPY src/ActionsImporter/ src/ActionsImporter/
RUN dotnet publish src/ActionsImporter/ActionsImporter.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/runtime:9.0
WORKDIR /app
RUN addgroup --system --gid 1000 appuser && adduser --system --uid 1000 --gid 1000 appuser
COPY --from=build --chown=appuser:appuser /app/publish .
USER appuser
ENTRYPOINT ["dotnet", "gh-actions-importer.dll"]
