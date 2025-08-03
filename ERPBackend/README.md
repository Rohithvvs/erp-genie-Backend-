# ERP Backend API

A complete .NET Core Web API backend for an ERP system built to serve a React.js frontend.

## Technology Stack

- **Backend Framework**: .NET Core 8.0 Web API using C#
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core (EF Core) for data access and migrations
- **Authentication**: JSON Web Tokens (JWT) for secure authentication
- **Hosting Target**: Render

## Features

### Authentication & User Management
- User registration with secure password hashing
- JWT-based authentication
- User profile management

### Invoice Management
- Create, read, update invoices
- Automatic stock deduction when creating invoices
- Pagination support for mobile optimization
- Invoice status management

### Inventory Management
- Complete CRUD operations for products
- Stock quantity tracking
- Product categorization and pricing

### Customer Management
- Customer profile management
- Customer-invoice relationships

## API Endpoints

### Authentication
- `POST /api/auth/register` - User registration
- `POST /api/auth/login` - User authentication

### Products
- `GET /api/products` - Get paginated list of products
- `GET /api/products/{id}` - Get single product
- `POST /api/products` - Create new product
- `PUT /api/products/{id}` - Update product
- `DELETE /api/products/{id}` - Delete product

### Invoices
- `GET /api/invoices` - Get paginated list of invoices
- `GET /api/invoices/{id}` - Get single invoice
- `POST /api/invoices` - Create new invoice
- `PUT /api/invoices/{id}` - Update invoice status

### Customers
- `GET /api/customers` - Get all customers
- `GET /api/customers/{id}` - Get single customer
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
- CreatedAt, UpdatedAt

### Customers Table
- Id (Primary Key)
- Name
- Address
- PhoneNumber
- GST
- Email
- CreatedAt, UpdatedAt

### Invoices Table
- Id (Primary Key)
- InvoiceNumber (Unique)
- InvoiceDate
- CustomerId (Foreign Key)
- UserId (Foreign Key)
- SubTotal
- TotalGST
- TotalAmount
- Status
- Notes
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

## Configuration

### Environment Variables for Render Deployment

Set the following environment variables in your Render dashboard:

1. **ConnectionStrings__DefaultConnection**
   ```
   Host=your-postgres-host;Database=erp_db;Username=your-username;Password=your-password
   ```

2. **JwtSettings__SecretKey**
   ```
   your-super-secret-key-with-at-least-32-characters
   ```

3. **JwtSettings__Issuer**
   ```
   ERPBackend
   ```

4. **JwtSettings__Audience**
   ```
   ERPFrontend
   ```

5. **JwtSettings__ExpirationHours**
   ```
   24
   ```

6. **CorsSettings__AllowedOrigins__0**
   ```
   http://localhost:3000
   ```

7. **CorsSettings__AllowedOrigins__1**
   ```
   https://your-app-name.vercel.app
   ```

## Local Development Setup

1. **Install Prerequisites**
   - .NET 8.0 SDK
   - PostgreSQL
   - Visual Studio Code or Visual Studio

2. **Database Setup**
   ```bash
   # Create PostgreSQL database
   createdb erp_db_dev
   
   # Update connection string in appsettings.Development.json
   ```

3. **Run the Application**
   ```bash
   cd ERPBackend
   dotnet restore
   dotnet run
   ```

4. **Access Swagger Documentation**
   - Navigate to `http://localhost:5000` or `https://localhost:5001`
   - Swagger UI will be available at the root URL

## Deployment to Render

1. **Create a new Web Service on Render**
   - Connect your GitHub repository
   - Set the build command: `dotnet publish -c Release -o out`
   - Set the start command: `dotnet out/ERPBackend.dll`

2. **Configure Environment Variables**
   - Add all the environment variables listed above
   - Ensure your PostgreSQL connection string points to your Render PostgreSQL database

3. **Deploy**
   - Render will automatically build and deploy your application
   - The API will be available at your Render URL

## Security Features

- **Password Hashing**: BCrypt for secure password storage
- **JWT Authentication**: Secure token-based authentication
- **CORS Configuration**: Flexible CORS policy for frontend integration
- **Input Validation**: Data annotations and model state validation
- **SQL Injection Protection**: Entity Framework Core parameterized queries

## API Documentation

The API includes automatic Swagger/OpenAPI documentation. When running locally, visit the root URL to access the interactive API documentation.

### Authentication Flow

1. **Register**: `POST /api/auth/register`
   ```json
   {
     "username": "shopowner",
     "password": "securepassword",
     "shopName": "My Shop",
     "address": "123 Main St",
     "gst": "GST123456789",
     "phoneNumber": "1234567890"
   }
   ```

2. **Login**: `POST /api/auth/login`
   ```json
   {
     "username": "shopowner",
     "password": "securepassword"
   }
   ```

3. **Use JWT Token**: Include the returned token in the Authorization header
   ```
   Authorization: Bearer <your-jwt-token>
   ```

## Error Handling

The API returns appropriate HTTP status codes and error messages:

- `200 OK` - Successful operation
- `201 Created` - Resource created successfully
- `400 Bad Request` - Invalid input data
- `401 Unauthorized` - Authentication required
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server error

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## License

This project is licensed under the MIT License.