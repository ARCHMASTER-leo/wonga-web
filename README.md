# Wonga Authentication System

## Tech Stack
- React (Frontend)
- .NET 9 Web API
- PostgreSQL
- Docker Compose

## Architecture
- Clean service separation
- JWT-based authentication
- BCrypt password hashing
- Integration tests included

## Running the Project

### Start Docker
docker compose up --build

### Run Tests
dotnet test

Frontend:
npm install
npm run dev

## API Endpoints

POST /api/auth/register
POST /api/auth/login
GET  /api/auth/me (Authorized)
POST /api/auth/change-password (Authorized)
PUT  /api/auth/update(Authorized)

## Security Features
- Password hashing with BCrypt
- JWT authentication
- Protected routes
- Claim-based identity


## Running Locally (Without Docker)
If you prefer to run the project outside of Docker during development:
Prerequisites

.NET 9 SDK
Node.js
PostgreSQL running locally or via Docker

1. Start the database only:
bashdocker compose up postgres -d
2. Update appsettings.Development.json in backend/Wonga.Api:
json{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=wonga_db;Username=wonga;Password=wonga123"
  },
  "Jwt": {
    "Key": "SUPER_SECRET_DEVELOPMENT_KEY_12345",
    "Issuer": "WongaApi",
    "Audience": "WongaClient"
  }
}
3. Run the API:
bashcd backend/Wonga.Api
dotnet run
```
API will be available at `http://localhost:5030/api`

**4. Update the frontend `.env`** in the `frontend` directory:
```
VITE_API_URL=http://localhost:5030/api
5. Run the frontend:
bashcd frontend
npm install
npm run dev
Frontend will be available at http://localhost:5173