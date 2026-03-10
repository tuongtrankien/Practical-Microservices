# PowerShell script to start all microservices
# This script starts your existing containers and runs docker-compose

Write-Host "🚀 Starting Practical Microservices..." -ForegroundColor Cyan
Write-Host ""

# Step 1: Start existing infrastructure containers
Write-Host "📦 Starting existing infrastructure containers..." -ForegroundColor Yellow
docker start sqlserver 2>$null
if ($LASTEXITCODE -eq 0) {
    Write-Host "   ✅ SQL Server started" -ForegroundColor Green
} else {
    Write-Host "   ⚠️  SQL Server container not found or already running" -ForegroundColor DarkYellow
}

docker start messaging-qnbztnvt 2>$null
if ($LASTEXITCODE -eq 0) {
    Write-Host "   ✅ RabbitMQ started" -ForegroundColor Green
} else {
    Write-Host "   ⚠️  RabbitMQ container not found or already running" -ForegroundColor DarkYellow
}

Write-Host ""
Write-Host "⏳ Waiting 5 seconds for infrastructure to be ready..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

# Step 2: Build and start microservices with docker-compose
Write-Host ""
Write-Host "🏗️  Building and starting microservices..." -ForegroundColor Yellow
docker-compose up --build -d

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "✅ All services started successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "📋 Service URLs:" -ForegroundColor Cyan
    Write-Host "   UserService:          http://localhost:5001/swagger" -ForegroundColor White
    Write-Host "   ProductService:       http://localhost:5002/swagger" -ForegroundColor White
    Write-Host "   OrderService:         http://localhost:5003/swagger" -ForegroundColor White
    Write-Host "   PaymentService:       http://localhost:5004/swagger" -ForegroundColor White
    Write-Host "   NotificationService:  http://localhost:5005/swagger" -ForegroundColor White
    Write-Host "   OrchestratorService:  http://localhost:5006/swagger" -ForegroundColor White
    Write-Host ""
    Write-Host "   RabbitMQ Management:  http://localhost:15672" -ForegroundColor White
    Write-Host "   (username: guest, password: guest)" -ForegroundColor DarkGray
    Write-Host ""
    Write-Host "💡 Tips:" -ForegroundColor Cyan
    Write-Host "   - View logs: docker-compose logs -f [service-name]" -ForegroundColor White
    Write-Host "   - Stop all:  docker-compose down" -ForegroundColor White
    Write-Host "   - Or use:    .\stop-all.ps1" -ForegroundColor White
} else {
    Write-Host ""
    Write-Host "❌ Failed to start services. Check errors above." -ForegroundColor Red
    exit 1
}
