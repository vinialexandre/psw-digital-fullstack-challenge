# PSW Digital - Desafio Técnico Fullstack

[Frontend]
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=vinialexandre_psw-digital-fullstack-challenge-frontend&metric=coverage)](https://sonarcloud.io/summary/new_code?id=vinialexandre_psw-digital-fullstack-challenge-frontend)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=vinialexandre_psw-digital-fullstack-challenge-frontend&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=vinialexandre_psw-digital-fullstack-challenge-frontend)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=vinialexandre_psw-digital-fullstack-challenge-frontend&metric=bugs)](https://sonarcloud.io/summary/new_code?id=vinialexandre_psw-digital-fullstack-challenge-frontend)

[Backend]
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=vinialexandre_psw-digital-fullstack-challenge-backend&metric=coverage)](https://sonarcloud.io/summary/new_code?id=vinialexandre_psw-digital-fullstack-challenge-backend)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=vinialexandre_psw-digital-fullstack-challenge-backend&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=vinialexandre_psw-digital-fullstack-challenge-backend)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=vinialexandre_psw-digital-fullstack-challenge-backend&metric=bugs)](https://sonarcloud.io/summary/new_code?id=vinialexandre_psw-digital-fullstack-challenge-backend)

API RESTful para consulta de feriados brasileiros com autenticacao JWT e cache distribuido.

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

## Fluxo de Requisicoes

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

### 2. Autenticacao (POST /api/auth/login)

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
- **Primeira requisicao**: Dados nao existem no Redis → Busca na BrasilAPI → Armazena no Redis
- **Requisicoes seguintes**: Dados existem no Redis → Retorna direto do cache (muito mais rapido)

### Expiracao:
- Cache expira em **24 horas** (configuravel)
- Apos expiracao, proxima requisicao busca novamente da API externa

### Fallback:
- Se Redis estiver indisponivel, usa **IMemoryCache** (cache em memoria)
- Aplicacao continua funcionando mesmo sem Redis

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

### Com Docker Compose (Recomendado)

```bash
cd backend
docker-compose up -d
```

Servicos disponiveis:
- API: http://localhost:5129
- Redis: localhost:6379
- Swagger: http://localhost:5129/swagger

### Localmente

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

## Endpoints Principais

### API
- `GET /api/holidays` - Lista feriados (requer autenticacao)
- `POST /api/auth/login` - Autenticacao JWT

### Health Checks
- `GET /health` - Status completo (aplicacao + Redis)
- `GET /health/ready` - Readiness probe
- `GET /health/live` - Liveness probe

## Testes

```bash
cd backend
dotnet test
```


