# Multi-stage build for PaymentService
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# Copy Shared.Contracts
COPY ["Shared.Contracts/Shared.Contracts.csproj", "Shared.Contracts/"]
# Copy PaymentService projects
COPY ["PaymentService/PaymentService.API/PaymentService.API.csproj", "PaymentService/PaymentService.API/"]
COPY ["PaymentService/PaymentService.Application/PaymentService.Application.csproj", "PaymentService/PaymentService.Application/"]
COPY ["PaymentService/PaymentService.Infrastructure/PaymentService.Infrastructure.csproj", "PaymentService/PaymentService.Infrastructure/"]
COPY ["PaymentService/PaymentService.Domain/PaymentService.Domain.csproj", "PaymentService/PaymentService.Domain/"]
RUN dotnet restore "PaymentService/PaymentService.API/PaymentService.API.csproj"
# Copy source files
COPY Shared.Contracts/. Shared.Contracts/
COPY PaymentService/. PaymentService/
WORKDIR "/src/PaymentService/PaymentService.API"
RUN dotnet build "PaymentService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PaymentService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PaymentService.API.dll"]
