Write-Host "Executando testes do frontend com cobertura de código..." -ForegroundColor Cyan

$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptPath

Write-Host "`nVerificando dependências..." -ForegroundColor Yellow
if (-not (Test-Path "node_modules")) {
    Write-Host "Instalando dependências..." -ForegroundColor Yellow
    npm install
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "`nErro ao instalar dependências!" -ForegroundColor Red
        exit 1
    }
}

Write-Host "`nExecutando testes com cobertura..." -ForegroundColor Yellow
npm run test:coverage

if ($LASTEXITCODE -ne 0) {
    Write-Host "`nTestes falharam!" -ForegroundColor Red
    exit 1
}

Write-Host "`nRelatórios de cobertura gerados em:" -ForegroundColor Green
Write-Host "  frontend/coverage/lcov.info (para SonarCloud)" -ForegroundColor White
Write-Host "  frontend/coverage/index.html (visualização local)" -ForegroundColor White

Write-Host "`nPara visualizar o relatório HTML, abra:" -ForegroundColor Cyan
Write-Host "  start frontend/coverage/index.html" -ForegroundColor White

