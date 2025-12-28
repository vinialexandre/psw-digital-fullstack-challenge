#!/bin/bash

echo -e "\033[0;36mExecutando testes do frontend com cobertura de código...\033[0m"

echo -e "\n\033[0;33mVerificando dependências...\033[0m"
if [ ! -d "node_modules" ]; then
    echo -e "\033[0;33mInstalando dependências...\033[0m"
    npm install
    
    if [ $? -ne 0 ]; then
        echo -e "\n\033[0;31mErro ao instalar dependências!\033[0m"
        exit 1
    fi
fi

echo -e "\n\033[0;33mExecutando testes com cobertura...\033[0m"
npm run test:coverage

if [ $? -ne 0 ]; then
    echo -e "\n\033[0;31mTestes falharam!\033[0m"
    exit 1
fi

echo -e "\n\033[0;32mRelatórios de cobertura gerados em:\033[0m"
echo -e "  \033[0;37mfrontend/coverage/lcov.info (para SonarCloud)\033[0m"
echo -e "  \033[0;37mfrontend/coverage/index.html (visualização local)\033[0m"

echo -e "\n\033[0;36mPara visualizar o relatório HTML, abra:\033[0m"
echo -e "  \033[0;37mxdg-open frontend/coverage/index.html\033[0m"

