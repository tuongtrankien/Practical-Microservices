# Multi-stage build for OrchestratorService
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# Copy Shared.Contracts
COPY ["Shared.Contracts/Shared.Contracts.csproj", "Shared.Contracts/"]
# Copy OrchestratorService projects
COPY ["OrchestratorService/OrchestratorService.API/OrchestratorService.API.csproj", "OrchestratorService/OrchestratorService.API/"]
COPY ["OrchestratorService/OrchestratorService.Application/OrchestratorService.Application.csproj", "OrchestratorService/OrchestratorService.Application/"]
COPY ["OrchestratorService/OrchestratorService.Infrastructure/OrchestratorService.Infrastructure.csproj", "OrchestratorService/OrchestratorService.Infrastructure/"]
COPY ["OrchestratorService/OrchestratorService.Domain/OrchestratorService.Domain.csproj", "OrchestratorService/OrchestratorService.Domain/"]
RUN dotnet restore "OrchestratorService/OrchestratorService.API/OrchestratorService.API.csproj"
# Copy source files
COPY Shared.Contracts/. Shared.Contracts/
COPY OrchestratorService/. OrchestratorService/
WORKDIR "/src/OrchestratorService/OrchestratorService.API"
RUN dotnet build "OrchestratorService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OrchestratorService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrchestratorService.API.dll"]
