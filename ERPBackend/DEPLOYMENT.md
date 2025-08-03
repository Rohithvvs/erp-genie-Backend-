# Deployment Guide for Render

This guide provides step-by-step instructions for deploying the ERP Backend API to Render.

## Prerequisites

1. A GitHub repository containing the ERP Backend code
2. A Render account (free tier available)
3. A PostgreSQL database (can be created on Render)

## Step 1: Create a PostgreSQL Database on Render

1. **Log in to Render Dashboard**
   - Go to [render.com](https://render.com)
   - Sign in to your account

2. **Create a New PostgreSQL Database**
   - Click "New +" button
   - Select "PostgreSQL"
   - Choose a name for your database (e.g., "erp-database")
   - Select your preferred region
   - Choose the free plan (or paid plan for production)
   - Click "Create Database"

3. **Note Database Credentials**
   - After creation, note down:
     - Database URL
     - Username
     - Password
     - Database name

## Step 2: Create a Web Service

1. **Create New Web Service**
   - Click "New +" button
   - Select "Web Service"
   - Connect your GitHub repository
   - Select the repository containing the ERP Backend code

2. **Configure the Web Service**
   - **Name**: `erp-backend` (or your preferred name)
   - **Environment**: `Docker`
   - **Region**: Choose the same region as your database
   - **Branch**: `main` (or your default branch)
   - **Build Command**: `dotnet publish -c Release -o out`
   - **Start Command**: `dotnet out/ERPBackend.dll`

## Step 3: Configure Environment Variables

Add the following environment variables in your Render Web Service settings:

### Database Connection
```
ConnectionStrings__DefaultConnection
```
Value: Your PostgreSQL connection string from Step 1
```
Host=your-postgres-host;Database=your-database-name;Username=your-username;Password=your-password
```

### JWT Configuration
```
JwtSettings__SecretKey
```
Value: A secure random string (at least 32 characters)
```
your-super-secret-key-with-at-least-32-characters-here
```

```
JwtSettings__Issuer
```
Value:
```
ERPBackend
```

```
JwtSettings__Audience
```
Value:
```
ERPFrontend
```

```
JwtSettings__ExpirationHours
```
Value:
```
24
```

### CORS Configuration
```
CorsSettings__AllowedOrigins__0
```
Value: Your React frontend development URL
```
http://localhost:3000
```

```
CorsSettings__AllowedOrigins__1
```
Value: Your React frontend production URL
```
https://your-app-name.vercel.app
```

### Additional Configuration
```
ASPNETCORE_ENVIRONMENT
```
Value:
```
Production
```

```
ASPNETCORE_URLS
```
Value:
```
http://0.0.0.0:10000
```

## Step 4: Deploy

1. **Save Environment Variables**
   - Click "Save Changes" after adding all environment variables

2. **Deploy**
   - Click "Deploy" button
   - Render will automatically build and deploy your application
   - The build process may take 5-10 minutes

3. **Monitor Deployment**
   - Watch the build logs for any errors
   - Ensure the deployment completes successfully

## Step 5: Verify Deployment

1. **Check Application Status**
   - Your application should show "Live" status
   - Note the provided URL (e.g., `https://erp-backend.onrender.com`)

2. **Test the API**
   - Visit your application URL
   - You should see the Swagger UI documentation
   - Test the health endpoint: `GET /api/health` (if implemented)

3. **Test Authentication**
   - Use the Swagger UI to test registration and login endpoints
   - Verify JWT token generation works correctly

## Step 6: Update Frontend Configuration

Update your React frontend to use the new API URL:

```javascript
// In your frontend configuration
const API_BASE_URL = 'https://your-app-name.onrender.com';
```

## Troubleshooting

### Common Issues

1. **Build Failures**
   - Check that all dependencies are properly specified in `ERPBackend.csproj`
   - Ensure the Dockerfile is correctly configured
   - Review build logs for specific error messages

2. **Database Connection Issues**
   - Verify the connection string format
   - Ensure the database is accessible from your web service
   - Check that all environment variables are correctly set

3. **CORS Issues**
   - Verify CORS origins are correctly configured
   - Ensure your frontend URL is included in the allowed origins

4. **JWT Issues**
   - Ensure the JWT secret key is at least 32 characters
   - Verify issuer and audience settings match your configuration

### Logs and Debugging

1. **View Application Logs**
   - Go to your web service dashboard
   - Click on "Logs" tab
   - Monitor real-time logs for errors

2. **Database Logs**
   - Check your PostgreSQL database logs
   - Verify connection attempts and any errors

## Production Considerations

1. **Security**
   - Use strong, unique JWT secret keys
   - Regularly rotate secrets
   - Enable HTTPS (automatic on Render)
   - Consider rate limiting for production

2. **Performance**
   - Monitor application performance
   - Consider upgrading to paid plans for better resources
   - Implement caching strategies

3. **Backup**
   - Set up regular database backups
   - Consider using Render's automated backup features

4. **Monitoring**
   - Set up health checks
   - Monitor application metrics
   - Configure alerts for downtime

## Cost Optimization

1. **Free Tier Limitations**
   - Free tier has limited resources
   - Applications may sleep after inactivity
   - Consider paid plans for production use

2. **Database Costs**
   - PostgreSQL databases have separate costs
   - Monitor usage to optimize costs

## Support

- **Render Documentation**: [docs.render.com](https://docs.render.com)
- **Render Support**: Available through the dashboard
- **Community**: Check Render's community forums for help

## Next Steps

After successful deployment:

1. Set up your React frontend to connect to the API
2. Test all functionality end-to-end
3. Set up monitoring and alerts
4. Plan for scaling as your application grows