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

### Prerequisites
- .NET 8.0 SDK
- PostgreSQL database
- Visual Studio 2022 or VS Code

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
   ```bash
   dotnet ef database update
   ```

5. **Run the application**
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