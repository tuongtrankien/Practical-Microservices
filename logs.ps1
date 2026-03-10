# PowerShell script to view logs for all or specific services

param(
    [string]$Service = ""
)

Write-Host "📋 Viewing logs..." -ForegroundColor Cyan
Write-Host ""

if ($Service -eq "") {
    Write-Host "Showing logs for all services (Ctrl+C to exit):" -ForegroundColor Yellow
    docker-compose logs -f
} else {
    Write-Host "Showing logs for $Service (Ctrl+C to exit):" -ForegroundColor Yellow
    docker-compose logs -f $Service
}
