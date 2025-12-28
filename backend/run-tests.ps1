Write-Host "Executando testes do backend..." -ForegroundColor Cyan

$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptPath

Write-Host "`nParando processos .NET que possam estar bloqueando arquivos..." -ForegroundColor Yellow
Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Where-Object { $_.Path -like "*\dotnet.exe" } | Stop-Process -Force -ErrorAction SilentlyContinue

Start-Sleep -Seconds 2

Write-Host "`nLimpando arquivos de build..." -ForegroundColor Yellow
dotnet clean --nologo

Write-Host "`nRestaurando dependencias..." -ForegroundColor Yellow
dotnet restore --nologo

if ($LASTEXITCODE -ne 0) {
    Write-Host "`nErro ao restaurar dependencias!" -ForegroundColor Red
    exit 1
}

Write-Host "`nExecutando testes..." -ForegroundColor Yellow
dotnet test --nologo --verbosity normal

if ($LASTEXITCODE -ne 0) {
    Write-Host "`nTestes falharam!" -ForegroundColor Red
    exit 1
}

Write-Host "`nTodos os testes passaram!" -ForegroundColor Green

