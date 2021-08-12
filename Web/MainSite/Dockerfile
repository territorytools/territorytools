FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

COPY Web/MainSite/Web.MainSite.csproj Web/MainSite/Web.MainSite.csproj
COPY Web/Data/Web.Data.csproj Web/Data/Web.Data.csproj
COPY Web/Entities/Web.Entities.csproj Web/Entities/Web.Entities.csproj
COPY Alba/Controllers/Alba.Controllers.csproj Alba/Controllers/Alba.Controllers.csproj

RUN dotnet restore Web/MainSite/Web.MainSite.csproj --packages /src/packages

COPY . .

RUN dotnet build Web/MainSite/Web.MainSite.csproj -c Release -o /app --no-restore

FROM build AS publish
RUN mkdir /app/publish && mkdir /app/publish/zh-Hant/
RUN dotnet publish Web/MainSite/Web.MainSite.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "TerritoryTools.Web.MainSite.dll"]

