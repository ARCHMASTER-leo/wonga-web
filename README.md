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