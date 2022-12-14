#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Services/Catalog.Service/Catalog.API.csproj", "src/Services/Catalog.Service/"]
COPY ["src/BuildingBlocks/CommandBus/CommandBus.csproj", "src/BuildingBlocks/CommandBus/"]
COPY ["src/BuildingBlocks/EventBus/EventBus.csproj", "src/BuildingBlocks/EventBus/"]
COPY ["src/BuildingBlocks/Utilities/Utilities.csproj", "src/BuildingBlocks/Utilities/"]
RUN dotnet restore "src/Services/Catalog.Service/Catalog.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Catalog.Service"
RUN dotnet build "Catalog.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Catalog.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Catalog.API.dll"]