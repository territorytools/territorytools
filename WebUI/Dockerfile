FROM mcr.microsoft.com/dotnet/core/aspnet AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk AS build
WORKDIR /src
COPY . /src

RUN dotnet build WebUI/WebUI.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish WebUI/WebUI.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

ENTRYPOINT ["dotnet", "WebUI.dll"]

