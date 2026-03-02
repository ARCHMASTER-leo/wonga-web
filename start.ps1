Write-Host "Stopping containers..." -ForegroundColor Yellow
docker compose down

Write-Host " Building images..." -ForegroundColor Yellow
docker compose build --no-cache

Write-Host " Starting backend + database..." -ForegroundColor Green
docker compose up -d postgres api

Write-Host " Waiting for API to start..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

Write-Host " Starting frontend..." -ForegroundColor Green
cd frontend
npm install
npm run dev