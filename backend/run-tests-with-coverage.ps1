Write-Host "Executando testes do backend com cobertura de código..." -ForegroundColor Cyan

$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptPath

Write-Host "`nParando processos .NET que possam estar bloqueando arquivos..." -ForegroundColor Yellow
Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Where-Object { $_.Path -like "*\dotnet.exe" } | Stop-Process -Force -ErrorAction SilentlyContinue

Start-Sleep -Seconds 2

Write-Host "`nLimpando arquivos de build..." -ForegroundColor Yellow
dotnet clean --nologo

Write-Host "`nRestaurando dependências..." -ForegroundColor Yellow
dotnet restore --nologo

if ($LASTEXITCODE -ne 0) {
    Write-Host "`nErro ao restaurar dependências!" -ForegroundColor Red
    exit 1
}

Write-Host "`nExecutando testes com cobertura..." -ForegroundColor Yellow
dotnet test `
    /p:CollectCoverage=true `
    /p:CoverletOutputFormat=opencover `
    /p:CoverletOutput=./coverage/ `
    /p:Exclude="[*]*.Program,[*]*.Migrations.*" `
    --verbosity normal

if ($LASTEXITCODE -ne 0) {
    Write-Host "`nTestes falhou!" -ForegroundColor Red
    exit 1
}

Write-Host "`nRelatório de cobertura gerado em:" -ForegroundColor Green
Write-Host "  backend/tests/HolidaysAPI.Tests/coverage/coverage.opencover.xml" -ForegroundColor White

Write-Host "`nPara visualizar o relatório, você pode usar ferramentas como:" -ForegroundColor Cyan
Write-Host "  - ReportGenerator: dotnet tool install -g dotnet-reportgenerator-globaltool" -ForegroundColor White
Write-Host "  - Comando: reportgenerator -reports:**/coverage.opencover.xml -targetdir:coverage-report -reporttypes:Html" -ForegroundColor White

