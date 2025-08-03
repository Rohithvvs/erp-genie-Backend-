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
=======
# ERP Backend API

A complete .NET Core Web API backend for an ERP system designed to serve a React.js frontend. This API provides comprehensive functionality for inventory management, invoice processing, customer management, and user authentication.

## Technology Stack

- **Backend Framework**: .NET Core 8.0 Web API using C#
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core (EF Core) for data access and migrations
- **Authentication**: JSON Web Tokens (JWT) for secure authentication
- **Hosting Target**: Render
- **API Documentation**: Swagger/OpenAPI

## Features

### Authentication & User Management
- User registration with secure password hashing
- JWT-based authentication
- User profile management with shop details

### Inventory Management
- Product CRUD operations
- Stock quantity tracking
- Product categorization
- Search and pagination

### Invoice Management
- Create invoices with multiple items
- Automatic stock deduction
- GST calculation
- Invoice status tracking
- Pagination for mobile optimization

### Customer Management
- Customer CRUD operations
- Customer search functionality
- Integration with invoice system

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Authenticate user and get JWT token

### Products
- `GET /api/products` - Get paginated list of products
- `GET /api/products/{id}` - Get specific product
- `POST /api/products` - Create new product
- `PUT /api/products/{id}` - Update product
- `DELETE /api/products/{id}` - Delete product

### Invoices
- `GET /api/invoices` - Get paginated list of invoices
- `GET /api/invoices/{id}` - Get specific invoice
- `POST /api/invoices` - Create new invoice
- `PUT /api/invoices/{id}` - Update invoice
- `DELETE /api/invoices/{id}` - Delete invoice

### Customers
- `GET /api/customers` - Get paginated list of customers
- `GET /api/customers/{id}` - Get specific customer
- `POST /api/customers` - Create new customer
- `PUT /api/customers/{id}` - Update customer
- `DELETE /api/customers/{id}` - Delete customer

## Database Schema

### Users Table
- Id (Primary Key)
- Username (Unique)
- PasswordHash
- ShopName
- Address
- GST
- PhoneNumber
- CreatedAt, UpdatedAt

### Products Table
- Id (Primary Key)
- ItemName
- StockQuantity
- Case
- Price
- GST
- Description
- Category
- CreatedAt, UpdatedAt

### Customers Table
- Id (Primary Key)
- Name
- PhoneNumber
- Address
- GST
- Email
- CreatedAt, UpdatedAt

### Invoices Table
- Id (Primary Key)
- InvoiceNumber (Unique)
- CustomerId (Foreign Key)
- UserId (Foreign Key)
- SubTotal
- GSTAmount
- TotalAmount
- Status
- Notes
- InvoiceDate
- CreatedAt, UpdatedAt

### InvoiceItems Table
- Id (Primary Key)
- InvoiceId (Foreign Key)
- ProductId (Foreign Key)
- ItemName
- Quantity
- Case
- Price
- GST
- TotalPrice
- CreatedAt

## Getting Started
=======
A comprehensive .NET Core Web API backend for an ERP (Enterprise Resource Planning) system built to serve a React.js frontend. This API provides complete functionality for user authentication, product inventory management, and invoice processing.

## Technology Stack

- **Framework**: .NET Core 8.0 Web API
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core (EF Core)
- **Authentication**: JSON Web Tokens (JWT)
- **Documentation**: Swagger/OpenAPI
- **Hosting**: Optimized for Render deployment

## Features

### 🔐 Authentication & User Management
- User registration with complete shop details
- Secure password hashing using BCrypt
- JWT token-based authentication
- Protected API endpoints

### 📦 Inventory Management
- Product CRUD operations
- Stock quantity tracking
- Category management
- SKU-based product identification
- Low stock alerts

### 🧾 Invoice Management
- Create invoices with customer and item details
- Automatic stock deduction
- Invoice number generation
- Status tracking (Pending, Paid, Cancelled)
- Dashboard analytics

## API Endpoints

### Authentication Endpoints
```
POST /api/auth/register    - Register a new user
POST /api/auth/login       - Authenticate user and get JWT token
GET  /api/auth/validate    - Validate JWT token
```

### Product Endpoints
```
GET    /api/products              - Get paginated products list
GET    /api/products/{id}         - Get single product
POST   /api/products              - Create new product
PUT    /api/products/{id}         - Update product
DELETE /api/products/{id}         - Delete product
GET    /api/products/categories   - Get product categories
GET    /api/products/low-stock    - Get low stock products
```

### Invoice Endpoints
```
GET  /api/invoices           - Get paginated invoices list
GET  /api/invoices/{id}      - Get single invoice
POST /api/invoices           - Create new invoice
PUT  /api/invoices/{id}      - Update invoice status/details
GET  /api/invoices/dashboard - Get dashboard analytics
```

## Setup and Installation

### Prerequisites
- .NET 8.0 SDK
- PostgreSQL database
- Visual Studio 2022 or VS Code
=======
- Git

### Local Development Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd ERPBackend
   ```

2. **Configure the database connection**
   Update the connection string in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=erp_db;Username=your_username;Password=your_password"
     }
   }
   ```

3. **Configure JWT settings**
   Update the JWT settings in `appsettings.json`:
   ```json
   {
     "JwtSettings": {
       "SecretKey": "your-super-secret-key-with-at-least-32-characters",
       "Issuer": "ERPBackend",
       "Audience": "ERPFrontend",
       "ExpirationHours": 24
     }
   }
   ```

4. **Run database migrations**
=======
2. **Install dependencies**
   ```bash
   dotnet restore
   ```

3. **Configure database connection**
   
   Update `appsettings.json` with your PostgreSQL connection string:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=ERPDatabase;Username=your_username;Password=your_password"
     }
   }
   ```

4. **Update JWT settings**
   
   Change the JWT secret key in `appsettings.json`:
   ```json
   {
     "JwtSettings": {
       "SecretKey": "your-super-secret-key-minimum-32-characters-long",
       "Issuer": "ERPBackend",
       "Audience": "ERPFrontend",
       "ExpirationInHours": 24
     }
   }
   ```

5. **Run database migrations**
   ```bash
   dotnet ef database update
   ```

5. **Run the application**
=======
6. **Run the application**
   ```bash
   dotnet run
   ```

6. **Access the API documentation**
   Navigate to `http://localhost:5000` to access Swagger UI

### Environment Variables for Production

When deploying to Render, set the following environment variables:

- `ASPNETCORE_ENVIRONMENT`: Production
- `ConnectionStrings__DefaultConnection`: Your PostgreSQL connection string
- `JwtSettings__SecretKey`: A secure secret key (at least 32 characters)
- `JwtSettings__Issuer`: ERPBackend
- `JwtSettings__Audience`: ERPFrontend
- `JwtSettings__ExpirationHours`: 24
- `CorsOrigins__0`: Your production frontend URL
- `CorsOrigins__1`: Your development frontend URL

## Deployment to Render

### Option 1: Using render.yaml (Recommended)

1. Push your code to a Git repository
2. Connect your repository to Render
3. Render will automatically detect the `render.yaml` file and configure the service

### Option 2: Manual Configuration

1. Create a new Web Service on Render
2. Connect your Git repository
3. Configure the following settings:
   - **Environment**: .NET
   - **Build Command**: `dotnet publish -c Release -o out`
   - **Start Command**: `dotnet out/ERPBackend.dll`
   - **Environment Variables**: Set all required variables as listed above

4. Create a PostgreSQL database on Render and link it to your service

## API Usage Examples

### Register a new user
=======
7. **Access Swagger documentation**
   
   Open your browser and navigate to: `https://localhost:5001` or `http://localhost:5000`

## Deployment to Render

### Step 1: Prepare Your Repository

1. Ensure your code is pushed to a Git repository (GitHub, GitLab, etc.)
2. Make sure all files including `appsettings.json` are committed

### Step 2: Create PostgreSQL Database on Render

1. Go to [Render Dashboard](https://dashboard.render.com)
2. Click "New +" and select "PostgreSQL"
3. Configure your database:
   - **Name**: `erp-database`
   - **User**: `erp_user`
   - **Region**: Choose your preferred region
4. Click "Create Database"
5. Note down the connection details (Internal Database URL)

### Step 3: Deploy the Web Service

1. In Render Dashboard, click "New +" and select "Web Service"
2. Connect your Git repository
3. Configure the service:

   **Basic Settings:**
   - **Name**: `erp-backend-api`
   - **Region**: Same as your database
   - **Branch**: `main` (or your primary branch)
   - **Root Directory**: Leave blank if the project is in the root
   - **Runtime**: `Docker` or `.NET`

   **Build Settings:**
   - **Build Command**: `dotnet publish -c Release -o out`
   - **Start Command**: `dotnet out/ERPBackend.dll`

   **Environment Variables:**
   Add the following environment variables:

   ```
   ASPNETCORE_ENVIRONMENT=Production
   ConnectionStrings__DefaultConnection=[Your PostgreSQL Internal Database URL]
   JwtSettings__SecretKey=[Your 32+ character secret key]
   JwtSettings__Issuer=ERPBackend
   JwtSettings__Audience=ERPFrontend
   JwtSettings__ExpirationInHours=24
   ```

   **Advanced Settings:**
   - **Port**: `5000` (or leave default)
   - **Health Check Path**: `/swagger/index.html`

4. Click "Create Web Service"

### Step 4: Configure CORS for Production

Update your frontend URL in the CORS configuration. In `Program.cs`, add your production frontend URL:

```csharp
policy.WithOrigins(
    "http://localhost:3000",      // React development server
    "https://localhost:3000",     // React development server (HTTPS)
    "https://your-frontend-app.vercel.app",  // Your actual frontend URL
    "https://*.vercel.app",       // Vercel production URLs
    "https://*.netlify.app"       // Netlify production URLs
)
```

### Step 5: Database Migration

After deployment, the database will be automatically created when the application starts. The `Program.cs` includes code to ensure the database is created:

```csharp
context.Database.EnsureCreated();
```

### Environment Variables Reference

| Variable | Description | Example |
|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | Application environment | `Production` |
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection string | `postgres://user:pass@host:5432/db` |
| `JwtSettings__SecretKey` | JWT signing key (32+ chars) | `your-super-secret-key-here` |
| `JwtSettings__Issuer` | JWT issuer claim | `ERPBackend` |
| `JwtSettings__Audience` | JWT audience claim | `ERPFrontend` |
| `JwtSettings__ExpirationInHours` | Token expiration time | `24` |

## API Usage Examples

### Register a New User

```bash
curl -X POST "https://your-api-url/api/auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "shopowner",
    "password": "securepassword123",
    "shopName": "My Shop",
    "address": "123 Main Street, City",
    "gst": "GST123456789",
    "phoneNumber": "9876543210"
  }'
```

### Login
=======
    "username": "john_doe",
    "password": "securePassword123",
    "shopName": "John'\''s Electronics",
    "address": "123 Main St, City, State",
    "gst": "GST123456789",
    "phoneNumber": "+1234567890"
  }'
```

### Login and Get Token

```bash
curl -X POST "https://your-api-url/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "shopowner",
    "password": "securepassword123"
  }'
```

### Create a product (with authentication)
```bash
curl -X POST "https://your-api-url/api/products" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "itemName": "Sample Product",
    "stockQuantity": 100,
    "case": "Box",
    "price": 50.00,
    "gst": 18.00,
    "description": "A sample product",
    "category": "Electronics"
  }'
```

### Create an invoice (with authentication)
```bash
curl -X POST "https://your-api-url/api/invoices" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "customerId": 1,
    "items": [
      {
        "productId": 1,
        "quantity": 5
      }
    ],
    "notes": "Sample invoice"
  }'
```

## Security Features

- **Password Hashing**: Uses BCrypt for secure password storage
- **JWT Authentication**: Secure token-based authentication
- **CORS Configuration**: Flexible CORS policy for frontend integration
- **Input Validation**: Comprehensive data validation using data annotations
- **SQL Injection Protection**: Entity Framework Core provides protection against SQL injection

## Error Handling

The API returns consistent error responses with appropriate HTTP status codes:

- `400 Bad Request`: Invalid input data
- `401 Unauthorized`: Invalid or missing authentication
- `404 Not Found`: Resource not found
- `500 Internal Server Error`: Server-side errors

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## License

This project is licensed under the MIT License.

## Support

For support and questions, please create an issue in the repository or contact the development team.
=======
    "username": "john_doe",
    "password": "securePassword123"
  }'
```

### Create a Product (with Authentication)

```bash
curl -X POST "https://your-api-url/api/products" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "itemName": "Laptop",
    "stockQuantity": 10,
    "case": "Electronics",
    "price": 999.99,
    "gst": 18.0,
    "description": "High-performance laptop",
    "category": "Electronics",
    "sku": "LAP001"
  }'
```

### Create an Invoice

```bash
curl -X POST "https://your-api-url/api/invoices" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "customer": {
      "name": "ABC Company",
      "address": "456 Business Ave",
      "phoneNumber": "+1987654321",
      "gst": "GST987654321"
    },
    "items": [
      {
        "productId": 1,
        "itemName": "Laptop",
        "quantity": 2,
        "case": "Electronics",
        "price": 999.99,
        "gst": 18.0
      }
    ],
    "dueDate": "2024-02-01T00:00:00Z",
    "notes": "Bulk order discount applied"
  }'
```

## Database Schema

### Users Table
- `Id` (Primary Key)
- `Username` (Unique)
- `PasswordHash`
- `ShopName`
- `Address`
- `GST`
- `PhoneNumber`
- `CreatedAt`, `UpdatedAt`

### Products Table
- `Id` (Primary Key)
- `ItemName`
- `StockQuantity`
- `Case`
- `Price`
- `GST`
- `Description`
- `Category`
- `SKU` (Unique)
- `CreatedAt`, `UpdatedAt`

### Customers Table
- `Id` (Primary Key)
- `Name`
- `Address`
- `PhoneNumber`
- `GST`
- `CreatedAt`, `UpdatedAt`

### Invoices Table
- `Id` (Primary Key)
- `InvoiceNumber` (Unique)
- `CustomerId` (Foreign Key)
- `InvoiceDate`
- `DueDate`
- `SubTotal`, `TotalGST`, `TotalAmount`
- `Status`
- `Notes`
- `CreatedAt`, `UpdatedAt`

### InvoiceItems Table
- `Id` (Primary Key)
- `InvoiceId` (Foreign Key)
- `ProductId` (Foreign Key)
- `ItemName`, `Quantity`, `Case`, `Price`, `GST`
- `LineTotal`
- `CreatedAt`

## Security Features

- **Password Hashing**: BCrypt with salt for secure password storage
- **JWT Authentication**: Stateless authentication with configurable expiration
- **CORS Protection**: Configured for specific frontend origins
- **Data Validation**: Comprehensive input validation on all endpoints
- **SQL Injection Protection**: EF Core parameterized queries
- **Rate Limiting**: Can be easily added with middleware

## Performance Optimizations

- **Pagination**: All list endpoints support pagination
- **Database Indexing**: Proper indexes on frequently queried columns
- **Async Operations**: All database operations are asynchronous
- **Connection Pooling**: Built-in with EF Core
- **Response Caching**: Can be easily added for read-heavy endpoints

## Monitoring and Logging

The application includes structured logging with different levels:
- **Information**: General application flow
- **Warning**: Unexpected but handled situations
- **Error**: Error conditions that need attention

## Support

For deployment issues or questions:
1. Check the Render deployment logs
2. Verify environment variables are set correctly
3. Ensure the PostgreSQL database is accessible
4. Check CORS configuration for frontend integration

## License

This project is licensed under the MIT License - see the LICENSE file for details.
