# Multi-stage build for ProductService
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# Copy Shared.Contracts
COPY ["Shared.Contracts/Shared.Contracts.csproj", "Shared.Contracts/"]
# Copy ProductService projects
COPY ["ProductService/ProductService.API/ProductService.API.csproj", "ProductService/ProductService.API/"]
COPY ["ProductService/ProductService.Application/ProductService.Application.csproj", "ProductService/ProductService.Application/"]
COPY ["ProductService/ProductService.Infrastructure/ProductService.Infrastructure.csproj", "ProductService/ProductService.Infrastructure/"]
COPY ["ProductService/ProductService.Domain/ProductService.Domain.csproj", "ProductService/ProductService.Domain/"]
RUN dotnet restore "ProductService/ProductService.API/ProductService.API.csproj"
# Copy source files
COPY Shared.Contracts/. Shared.Contracts/
COPY ProductService/. ProductService/
WORKDIR "/src/ProductService/ProductService.API"
RUN dotnet build "ProductService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProductService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductService.API.dll"]
