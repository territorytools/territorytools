FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src

COPY . /src

RUN dotnet restore Web/MainSite/Web.MainSite.csproj  

RUN dotnet build Web/MainSite/Web.MainSite.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Web/MainSite/Web.MainSite.csproj -c Release -o /app --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

ENTRYPOINT ["dotnet", "TerritoryTools.Web.MainSite.dll"]

