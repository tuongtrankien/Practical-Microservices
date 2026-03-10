# Multi-stage build for OrderService
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# Copy Shared.Contracts
COPY ["Shared.Contracts/Shared.Contracts.csproj", "Shared.Contracts/"]
# Copy OrderService projects
COPY ["OrderService/OrderService.API/OrderService.API.csproj", "OrderService/OrderService.API/"]
COPY ["OrderService/OrderService.Application/OrderService.Application.csproj", "OrderService/OrderService.Application/"]
COPY ["OrderService/OrderService.Infrastructure/OrderService.Infrastructure.csproj", "OrderService/OrderService.Infrastructure/"]
COPY ["OrderService/OrderService.Domain/OrderService.Domain.csproj", "OrderService/OrderService.Domain/"]
RUN dotnet restore "OrderService/OrderService.API/OrderService.API.csproj"
# Copy source files
COPY Shared.Contracts/. Shared.Contracts/
COPY OrderService/. OrderService/
WORKDIR "/src/OrderService/OrderService.API"
RUN dotnet build "OrderService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OrderService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderService.API.dll"]
