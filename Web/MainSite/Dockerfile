FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /src

COPY . /src
RUN dotnet restoreWeb/MainSite/Web.MainSite.csproj  

FROM build AS publish
RUN dotnet publish Web/MainSite/Web.MainSite.csproj -c Release -o /app --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

ENTRYPOINT ["dotnet", "TerritoryTools.Web.MainSite.dll"]

