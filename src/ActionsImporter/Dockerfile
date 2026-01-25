FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY src/Directory.Build.props src/
COPY src/ActionsImporter/ActionsImporter.csproj src/ActionsImporter/
RUN dotnet restore src/ActionsImporter/ActionsImporter.csproj
COPY src/ActionsImporter/ src/ActionsImporter/
RUN dotnet publish src/ActionsImporter/ActionsImporter.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/runtime:9.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "gh-actions-importer.dll"]
