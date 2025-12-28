# Configuração do SonarCloud

Este guia explica como configurar a análise de código com SonarCloud para o projeto.

## Pré-requisitos

1. Conta no [SonarCloud](https://sonarcloud.io/)
2. Organização criada no SonarCloud (ex: `vinialexandre`)
3. Repositório no GitHub

## Configuração Inicial

### 1. Criar Projetos no SonarCloud

Acesse [SonarCloud](https://sonarcloud.io/) e crie dois projetos:

**Backend:**
- Project Key: `vinialexandre_psw-digital-fullstack-challenge-backend`
- Organization: `vinialexandre`

**Frontend:**
- Project Key: `vinialexandre_psw-digital-fullstack-challenge-frontend`
- Organization: `vinialexandre`

### 2. Gerar Tokens de Acesso

Para cada projeto, gere um token de acesso:

1. Acesse **My Account** > **Security**
2. Clique em **Generate Tokens**
3. Nomeie os tokens:
   - `PSW Backend Token`
   - `PSW Frontend Token`
4. Copie os tokens gerados

### 3. Configurar Secrets no GitHub

No repositório do GitHub, adicione os tokens como secrets:

1. Acesse **Settings** > **Secrets and variables** > **Actions**
2. Clique em **New repository secret**
3. Adicione os seguintes secrets:
   - `SONAR_TOKEN_BACKEND`: Cole o token do backend
   - `SONAR_TOKEN_FRONTEND`: Cole o token do frontend

## Executar Análise Localmente

### Backend

```bash
cd backend

dotnet restore

dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=./coverage/

dotnet sonarscanner begin /k:"vinialexandre_psw-digital-fullstack-challenge-backend" /o:"vinialexandre" /d:sonar.host.url="https://sonarcloud.io" /d:sonar.token="SEU_TOKEN_BACKEND" /d:sonar.cs.opencover.reportsPaths="**/coverage.opencover.xml"

dotnet build

dotnet sonarscanner end /d:sonar.token="SEU_TOKEN_BACKEND"
```

### Frontend

```bash
cd frontend

npm install

npm run test:coverage

npx sonar-scanner \
  -Dsonar.projectKey=vinialexandre_psw-digital-fullstack-challenge-frontend \
  -Dsonar.organization=vinialexandre \
  -Dsonar.sources=. \
  -Dsonar.host.url=https://sonarcloud.io \
  -Dsonar.token=SEU_TOKEN_FRONTEND
```

## Análise Automática via GitHub Actions

A análise é executada automaticamente quando:
- Há push na branch `main`
- É aberto ou atualizado um Pull Request

O workflow está configurado em `.github/workflows/sonarcloud.yml`

## Visualizar Resultados

Acesse os dashboards no SonarCloud:

- **Backend**: https://sonarcloud.io/project/overview?id=vinialexandre_psw-digital-fullstack-challenge-backend
- **Frontend**: https://sonarcloud.io/project/overview?id=vinialexandre_psw-digital-fullstack-challenge-frontend

## Métricas de Cobertura

### Backend (.NET)

Executar testes com cobertura:
```bash
cd backend
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=./coverage/
```

Relatório gerado em: `backend/tests/HolidaysAPI.Tests/coverage/coverage.opencover.xml`

### Frontend (Next.js)

Executar testes com cobertura:
```bash
cd frontend
npm run test:coverage
```

Relatórios gerados em:
- `frontend/coverage/lcov.info` (para SonarCloud)
- `frontend/coverage/index.html` (visualização local)

## Limites de Cobertura

### Frontend
Configurado em `frontend/jest.config.js`:
- Branches: 50%
- Functions: 50%
- Lines: 50%
- Statements: 50%

### Backend
Configurado via Coverlet durante execução dos testes.

## Arquivos de Configuração

- `backend/sonar-project.properties` - Configuração do SonarCloud para backend
- `frontend/sonar-project.properties` - Configuração do SonarCloud para frontend
- `.github/workflows/sonarcloud.yml` - GitHub Actions workflow
- `backend/tests/HolidaysAPI.Tests/HolidaysAPI.Tests.csproj` - Configuração do Coverlet
- `frontend/jest.config.js` - Configuração de cobertura do Jest

## Troubleshooting

### Erro: "Project not found"
Verifique se o `projectKey` e `organization` estão corretos nos arquivos `sonar-project.properties`

### Erro: "Invalid token"
Verifique se os secrets `SONAR_TOKEN_BACKEND` e `SONAR_TOKEN_FRONTEND` estão configurados corretamente no GitHub

### Cobertura não aparece no SonarCloud
Verifique se os caminhos dos relatórios estão corretos:
- Backend: `**/coverage.opencover.xml`
- Frontend: `coverage/lcov.info`

