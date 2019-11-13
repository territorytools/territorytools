FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /src
COPY . /src

RUN dotnet build WebUI/WebUI.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish WebUI/WebUI.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

ENTRYPOINT ["dotnet", "WebUI.dll"]

