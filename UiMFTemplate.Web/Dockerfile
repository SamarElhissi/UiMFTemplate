#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["Warehouse.Web/Warehouse.Web.csproj", "Warehouse.Web/"]
COPY ["Warehouse.DependencyInjection/Warehouse.DependencyInjection.csproj", "Warehouse.DependencyInjection/"]
COPY ["Warehouse.Notifications/Warehouse.Notifications.csproj", "Warehouse.Notifications/"]
COPY ["Warehouse.Infrastructure/Warehouse.Infrastructure.csproj", "Warehouse.Infrastructure/"]
COPY ["Warehouse.Filing/Warehouse.Filing.csproj", "Warehouse.Filing/"]
COPY ["Warehouse.Users/Warehouse.Users.csproj", "Warehouse.Users/"]
COPY ["Warehouse.Help/Warehouse.Help.csproj", "Warehouse.Help/"]
COPY ["Warehouse.Core/Warehouse.Core.csproj", "Warehouse.Core/"]
COPY ["Warehouse.Excel/Warehouse.Excel.csproj", "Warehouse.Excel/"]
COPY ["Warehouse.App.EventNotification/Warehouse.App.EventNotification.csproj", "Warehouse.App.EventNotification/"]
COPY ["Warehouse.DataSeed/Warehouse.DataSeed.csproj", "Warehouse.DataSeed/"]
RUN dotnet restore "Warehouse.Web/Warehouse.Web.csproj"
COPY . .
WORKDIR "/src/Warehouse.Web"
RUN dotnet build "Warehouse.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Warehouse.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Warehouse.Web.dll"]