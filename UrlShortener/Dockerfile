FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /src
COPY ./UrlShortener /src/UrlShortener
COPY ./Data /src/Data
COPY ./Entities /src/Entities

RUN dotnet build UrlShortener/UrlShortener.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish UrlShortener/UrlShortener.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

ENTRYPOINT ["dotnet", "UrlShortener.dll"]

