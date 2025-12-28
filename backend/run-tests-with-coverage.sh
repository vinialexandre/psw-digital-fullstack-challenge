#!/bin/bash

echo -e "\033[0;36mExecutando testes do backend com cobertura de código...\033[0m"

echo -e "\n\033[0;33mRestaurando dependências...\033[0m"
dotnet restore

if [ $? -ne 0 ]; then
    echo -e "\n\033[0;31mErro ao restaurar dependências!\033[0m"
    exit 1
fi

echo -e "\n\033[0;33mExecutando testes com cobertura...\033[0m"
dotnet test \
    /p:CollectCoverage=true \
    /p:CoverletOutputFormat=opencover \
    /p:CoverletOutput=./coverage/ \
    /p:Exclude="[*]*.Program,[*]*.Migrations.*" \
    --verbosity normal

if [ $? -ne 0 ]; then
    echo -e "\n\033[0;31mTestes falharam!\033[0m"
    exit 1
fi

echo -e "\n\033[0;32mRelatório de cobertura gerado em:\033[0m"
echo -e "  \033[0;37mbackend/tests/HolidaysAPI.Tests/coverage/coverage.opencover.xml\033[0m"

echo -e "\n\033[0;36mPara visualizar o relatório, você pode usar ferramentas como:\033[0m"
echo -e "  \033[0;37m- ReportGenerator: dotnet tool install -g dotnet-reportgenerator-globaltool\033[0m"
echo -e "  \033[0;37m- Comando: reportgenerator -reports:**/coverage.opencover.xml -targetdir:coverage-report -reporttypes:Html\033[0m"

