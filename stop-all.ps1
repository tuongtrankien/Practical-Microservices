# PowerShell script to stop all microservices

Write-Host "🛑 Stopping all microservices..." -ForegroundColor Cyan
Write-Host ""

# Stop docker-compose services
Write-Host "Stopping Docker Compose services..." -ForegroundColor Yellow
docker-compose down

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ All microservices stopped" -ForegroundColor Green
} else {
    Write-Host "⚠️  Some services may not have stopped correctly" -ForegroundColor DarkYellow
}

Write-Host ""
Write-Host "💡 Note: Your SQL Server and RabbitMQ containers are still running" -ForegroundColor Cyan
Write-Host "   To stop them manually:" -ForegroundColor White
Write-Host "   docker stop sqlserver messaging-qnbztnvt" -ForegroundColor DarkGray
Write-Host ""
