# PSW Digital - Desafio Técnico Fullstack

[Frontend]
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=vinialexandre_psw-digital-fullstack-challenge-frontend&metric=coverage)](https://sonarcloud.io/summary/new_code?id=vinialexandre_psw-digital-fullstack-challenge-frontend)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=vinialexandre_psw-digital-fullstack-challenge-frontend&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=vinialexandre_psw-digital-fullstack-challenge-frontend)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=vinialexandre_psw-digital-fullstack-challenge-frontend&metric=bugs)](https://sonarcloud.io/summary/new_code?id=vinialexandre_psw-digital-fullstack-challenge-frontend)

[Backend]
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=vinialexandre_psw-digital-fullstack-challenge-backend&metric=coverage)](https://sonarcloud.io/summary/new_code?id=vinialexandre_psw-digital-fullstack-challenge-backend)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=vinialexandre_psw-digital-fullstack-challenge-backend&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=vinialexandre_psw-digital-fullstack-challenge-backend)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=vinialexandre_psw-digital-fullstack-challenge-backend&metric=bugs)](https://sonarcloud.io/summary/new_code?id=vinialexandre_psw-digital-fullstack-challenge-backend)

API RESTfull para consulta de feriados brasileiros com autenticacao JWT e cache distribuido.

## Screenshots

### Tela de Login
<img width="1918" height="907" alt="image" src="https://github.com/user-attachments/assets/f415be16-4493-4b42-85fd-68db85805cfe" />

### Tela Principal
<img width="1918" height="907" alt="image" src="https://github.com/user-attachments/assets/3cfd5be5-b18f-468b-bc43-d40b3fd012c0" />

### Modal do Feriado
<img width="1918" height="908" alt="image" src="https://github.com/user-attachments/assets/ad144e44-2728-4ddf-9ead-efb3e4715334" />

### Busca

<img width="1918" height="905" alt="image" src="https://github.com/user-attachments/assets/a6012c6a-0ea5-47ba-9135-b33e634cf7ba" />



## Arquitetura

### Clean Architecture (Backend)

```
┌─────────────────────────────────────────────────────────┐
│                      API Layer                          │
│  Controllers, Middlewares, Health Checks, JWT Config    │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│                 Application Layer                       │
│     Services, DTOs, Interfaces (ICacheService)          │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│                  Domain Layer                           │
│          Entities, Domain Interfaces                    │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│              Infrastructure Layer                       │
│  RedisCacheService, BrasilApiService, Repositories      │
└─────────────────────────────────────────────────────────┘
```

## Fluxo de Requisições

### 1. Consulta de Feriados (GET /api/holidays)

```
Cliente → API → AuthMiddleware (JWT) → HolidayController
                                              ↓
                                        HolidayService
                                              ↓
                                    ┌─────────▼──────────┐
                                    │  RedisCacheService │
                                    └─────────┬──────────┘
                                              │
                        ┌─────────────────────┴──────────────────┐
                        │                                        │
                   Cache HIT                                Cache MISS
                        │                                        │
                  Retorna dados                          BrasilApiService
                   do Redis                                     │
                        │                              Busca API externa
                        │                                        │
                        │                              Armazena no Redis
                        │                              (expira em 24h)
                        │                                        │
                        └────────────────┬───────────────────────┘
                                         │
                                   Retorna JSON

```

### 2. Autenticação (POST /api/auth/login)

```
Cliente → API → AuthController → AuthService
                                      ↓
                              Valida credenciais
                                      ↓
                              Gera token JWT
                                      ↓
                            Retorna token + cookie
```

## Cache com Redis

### Quando armazenamos no cache:
- **Primeira requisição**: Dados não existem no Redis → Busca na BrasilAPI → Armazena no Redis
- **Requisições seguintes**: Dados existem no Redis → Retorna direto do cache (muito mais rápido)

### Expiração:
- Cache expira em **24 horas** (configurável)
- Após expiração, próxima requisição busca novamente da API externa

### Fallback:
- Se Redis estiver indisponível, usa **IMemoryCache** (cache em memória)
- Aplicação continua funcionando mesmo sem Redis

## Tecnologias

### Backend
- .NET 9
- Redis (StackExchange.Redis)
- ASP.NET Core Health Checks
- JWT Authentication
- Swagger/OpenAPI
- xUnit + FluentAssertions + Moq

### Frontend
- Next.js 15
- TypeScript
- Tailwind CSS
- Shadcn/ui

## Executando o Projeto

### Pre-requisitos
- Docker e Docker Compose
- Node.js 20+
- .NET 9 SDK (opcional, para desenvolvimento local sem Docker)

### Com VS Code (Recomendado)

1. Abra o projeto no VS Code
2. Selecione a configuracao **"Full Stack (Docker + Frontend)"** no menu de debug
3. Pressione **F5**

Isso ira:
- Subir o Redis e Backend via Docker Compose
- Iniciar o Frontend com hot-reload
- Abrir o navegador em http://localhost:3000

### Com Docker Compose (Terminal)

```bash
docker-compose up --build -d
cd frontend && npm install && npm run dev
```

### Servicos disponiveis
| Servico | URL |
|---------|-----|
| Frontend | http://localhost:3000 |
| Backend API | http://localhost:5129 |
| Swagger | http://localhost:5129/swagger |
| Redis | localhost:6379 |

### Parar os servicos

```bash
docker-compose down
```

### Desenvolvimento local (sem Docker)

#### Backend
```bash
cd backend/src/HolidaysAPI.API
dotnet run
```

#### Frontend
```bash
cd frontend
npm install
npm run dev
```

> **Nota**: Para rodar o backend localmente sem Docker, desabilite o Redis em `appsettings.Development.json` ou suba um Redis local.

## Endpoints Principais

### API
- `GET /api/holidays` - Lista feriados (requer autenticação)
- `POST /api/auth/login` - Autenticação JWT

### Health Checks
- `GET /health` - Status completo (aplicação + Redis)
- `GET /health/ready` - Readiness probe
- `GET /health/live` - Liveness probe

## Testes

```bash
cd backend
dotnet test
```


