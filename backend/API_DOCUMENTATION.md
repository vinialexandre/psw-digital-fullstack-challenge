# Holidays API Documentation

## Base URL
```
http://localhost:5000
https://localhost:5001
```

## Authentication

### Login
**POST** `/api/auth/login`

Request:
```json
{
  "username": "admin",
  "password": "admin123"
}
```

Response:
```json
{
  "success": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiresAt": "2025-12-29T15:00:00Z"
  },
  "message": "Login successful",
  "totalRecords": 0
}
```

## Holidays Endpoints

### Get Holidays
**GET** `/api/holidays`

**Headers:**
```
Authorization: Bearer {token}
```

**Query Parameters:**
- `date` (DateTime, optional): Filter by specific date
- `type` (string, optional): Filter by type ("National" or "Municipal")
- `searchTerm` (string, optional): Search by holiday name
- `sortBy` (string, optional): Sort field ("date", "name", "type")
- `sortDescending` (boolean, optional): Sort direction (default: false)

**Example Request:**
```
GET /api/holidays?type=National&sortBy=date&sortDescending=false
```

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "date": "01/01/2025",
      "name": "Confraternização Universal",
      "type": "National"
    },
    {
      "date": "21/04/2025",
      "name": "Tiradentes",
      "type": "National"
    }
  ],
  "message": "Holidays retrieved successfully",
  "totalRecords": 2
}
```

## Running the API

```bash
cd backend/src/HolidaysAPI.API
dotnet run
```

Access Swagger UI at: `https://localhost:5001/swagger`

## Running Tests

```bash
cd backend
dotnet test
```

