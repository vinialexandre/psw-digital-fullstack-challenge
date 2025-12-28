# Guia de Desenvolvimento

## Rodando o Projeto Localmente

### Opção 1: Usando VSCode (Recomendado para Debug)

**Terminal 1 - Frontend:**
```bash
cd frontend
npm run dev
```
O frontend estará disponível em: http://localhost:3000

**VSCode - Backend:**
1. Pressione **F5**
2. O backend iniciará em modo debug
3. O Swagger abrirá automaticamente em: https://localhost:5001/swagger

### Opção 2: Linha de Comando

**Terminal 1 - Backend:**
```bash
cd backend/src/HolidaysAPI.API
dotnet run
```

**Terminal 2 - Frontend:**
```bash
cd frontend
npm run dev
```

## Rodando Testes

**Backend:**
```bash
cd backend
dotnet test
```

**Frontend:**
```bash
cd frontend
npm test
```

## Credenciais de Acesso

- **Username:** admin
- **Password:** admin123

## Endpoints da API

- **Login:** POST http://localhost:5000/api/auth/login
- **Holidays:** GET http://localhost:5000/api/holidays
- **Swagger:** https://localhost:5001/swagger

## Portas

- Backend: 5000 (HTTP) / 5001 (HTTPS)
- Frontend: 3000

