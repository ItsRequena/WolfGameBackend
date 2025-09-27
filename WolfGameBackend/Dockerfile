# Etapa 1: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /App

# Copiar todo y restaurar dependencias
COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -f net8.0 -o out

# Etapa 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /App
COPY --from=build /App/out .
ENTRYPOINT ["dotnet", "WolfGameBackend.dll"]
