# ERP .NET Core Web API Backend

This repository contains a **.NET 8** Web API backend for an ERP system intended to power a React front-end.

## Tech Stack

- ASP.NET Core 8 Web API 
- Entity Framework Core 8 + Npgsql provider 
- PostgreSQL 
- JWT authentication 
- Swagger (Swashbuckle) 
- Hosted on **Render**

## Getting Started (Local)

1. **Prerequisites**  
   - .NET SDK 8.0+  
   - PostgreSQL ≥ 14  

2. **Clone & Restore**

```bash
 git clone <repo>
 cd <repo>
 dotnet restore
```

3. **Configure Environment**  
   Duplicate `appsettings.json` ➜ `appsettings.Development.json` and set:

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=erp_db;Username=postgres;Password=postgres"
  },
  "Jwt": {
    "Key": "YOUR_DEVELOPMENT_SECRET"
  }
}
```

4. **Database & Migrations**

```bash
 dotnet ef database update   # applies the included InitialCreate migration
```

5. **Run**

```bash
 dotnet run
```

Swagger UI ➜ https://localhost:5001/swagger

## Project Structure

```text
├── Controllers/        # API controllers
├── Data/               # EF Core DbContext & migrations
├── DTOs/               # Data Transfer Objects
├── Models/             # Entity models (Tables)
├── Services/           # Domain/helper services
├── Program.cs          # Application entry-point
└── ErpApi.csproj       # Project file
```

## Authentication Flow

1. `POST /api/auth/register` ➜ Register a user.  
2. `POST /api/auth/login`    ➜ Receive **JWT** token.  
3. All other endpoints require `Authorization: Bearer <token>` header.

## CORS

Only the following origins are allowed by default:

- `http://localhost:3000`  (React dev server)
- `https://your-app-name.vercel.app`  (Production front-end)

Add more origins in **Program.cs** ➜ CORS section if needed.

## Deploying to Render

1. **Create a new Web Service** in Render.  
   - **Environment → Dockerfile:** `dotnet` (native).  
   - **Build Command:** `dotnet build -c Release`  
   - **Start Command:** `dotnet ErpApi.dll` (Render auto-detects).  
   - **Region:** pick nearest.  

2. **Environment Variables**

   | Key | Value |
   |-----|-------|
   | `ConnectionStrings__Default` | `Host=<host>;Port=5432;Database=<db>;Username=<user>;Password=<pw>` |
   | `Jwt:Key` | `<LONG_RANDOM_SECRET>` |
   | `ASPNETCORE_URLS` | `http://0.0.0.0:10000` (Render default) |

3. **Database**  
   - Add a **PostgreSQL** instance in Render or point to an external DB.  
   - Apply migrations automatically: the app runs `Database.Migrate()` on start.

4. **Deploy**. Render will build & run your service.  
   Swagger available at `https://<service>.onrender.com/swagger`.

## Initial Migration Note

The `Data/Migrations/` folder contains the **InitialCreate** migration generated via:

```bash
 dotnet ef migrations add InitialCreate
```

If you modify models, create new migrations & deploy:

```bash
 dotnet ef migrations add <Name>
 git push
```

Render will apply them on next deploy.

---

Happy building! 🎉