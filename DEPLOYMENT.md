# Deployment Guide for Render

This guide provides step-by-step instructions for deploying the ERP Backend API to Render.

## Prerequisites

1. A GitHub, GitLab, or Bitbucket account with your ERP backend code
2. A Render account (free tier available)
3. PostgreSQL database (can be created on Render)

## Step 1: Prepare Your Repository

Ensure your repository contains all the necessary files:

- `ERPBackend.csproj` - Project file with dependencies
- `Program.cs` - Main application entry point
- `appsettings.json` - Configuration file
- `Dockerfile` - For containerized deployment
- `render.yaml` - Render deployment configuration
- All source code files in their respective directories

## Step 2: Create a Render Account

1. Go to [render.com](https://render.com)
2. Sign up for a free account
3. Verify your email address

## Step 3: Deploy Using render.yaml (Recommended)

### Option A: Automatic Deployment

1. **Connect your repository**
   - In Render dashboard, click "New +"
   - Select "Blueprint" (if you have render.yaml)
   - Connect your Git repository
   - Render will automatically detect the `render.yaml` file

2. **Review and deploy**
   - Render will show you the services and databases that will be created
   - Click "Apply" to start the deployment process

### Option B: Manual Service Creation

If you prefer to create services manually:

1. **Create PostgreSQL Database**
   - Click "New +" → "PostgreSQL"
   - Choose a name (e.g., "erp-postgres")
   - Select your preferred region
   - Choose a plan (free tier available)
   - Click "Create Database"

2. **Create Web Service**
   - Click "New +" → "Web Service"
   - Connect your Git repository
   - Configure the service:
     - **Name**: erp-backend
     - **Environment**: .NET
     - **Build Command**: `dotnet publish -c Release -o out`
     - **Start Command**: `dotnet out/ERPBackend.dll`
     - **Plan**: Free (or choose a paid plan)

3. **Configure Environment Variables**
   Add the following environment variables:

   ```
   ASPNETCORE_ENVIRONMENT=Production
   ConnectionStrings__DefaultConnection=<your-postgres-connection-string>
   JwtSettings__SecretKey=<your-secret-key>
   JwtSettings__Issuer=ERPBackend
   JwtSettings__Audience=ERPFrontend
   JwtSettings__ExpirationHours=24
   CorsOrigins__0=https://your-frontend-domain.vercel.app
   CorsOrigins__1=http://localhost:3000
   ```

## Step 4: Configure Environment Variables

### Database Connection String
Get your PostgreSQL connection string from the Render database dashboard. It will look like:
```
postgresql://username:password@host:port/database
```

### JWT Secret Key
Generate a secure secret key (at least 32 characters):
```bash
# Using OpenSSL
openssl rand -base64 32

# Or use a secure random string generator
```

### CORS Origins
Update the CORS origins to match your frontend URLs:
- Development: `http://localhost:3000`
- Production: `https://your-app-name.vercel.app`

## Step 5: Deploy and Test

1. **Deploy the service**
   - Click "Create Web Service"
   - Render will build and deploy your application
   - Monitor the build logs for any errors

2. **Test the deployment**
   - Once deployed, you'll get a URL like: `https://your-app-name.onrender.com`
   - Visit the URL to access Swagger UI
   - Test the API endpoints

3. **Verify database connection**
   - Check the application logs in Render dashboard
   - Ensure the database migration ran successfully

## Step 6: Configure Custom Domain (Optional)

1. **Add custom domain**
   - Go to your service settings
   - Click "Custom Domains"
   - Add your domain (e.g., `api.yourdomain.com`)

2. **Configure DNS**
   - Add a CNAME record pointing to your Render service URL
   - Wait for DNS propagation

## Step 7: Update Frontend Configuration

Update your React frontend to use the new API URL:

```javascript
// In your frontend configuration
const API_BASE_URL = 'https://your-app-name.onrender.com';
// or
const API_BASE_URL = 'https://api.yourdomain.com';
```

## Troubleshooting

### Common Issues

1. **Build Failures**
   - Check the build logs in Render dashboard
   - Ensure all dependencies are properly specified in `ERPBackend.csproj`
   - Verify the build command is correct

2. **Database Connection Issues**
   - Verify the connection string format
   - Check if the database is accessible from your service
   - Ensure the database user has proper permissions

3. **CORS Errors**
   - Verify CORS origins are correctly configured
   - Check that your frontend URL is included in the allowed origins

4. **JWT Authentication Issues**
   - Ensure the JWT secret key is properly set
   - Verify the issuer and audience match your configuration

### Logs and Monitoring

1. **View Application Logs**
   - Go to your service dashboard
   - Click "Logs" tab
   - Monitor for errors and warnings

2. **Database Logs**
   - Go to your database dashboard
   - Check connection logs and performance metrics

## Performance Optimization

### For Production Use

1. **Upgrade to Paid Plan**
   - Free tier has limitations (sleeps after inactivity)
   - Paid plans provide better performance and reliability

2. **Database Optimization**
   - Consider upgrading to a paid PostgreSQL plan
   - Monitor database performance and add indexes as needed

3. **Caching**
   - Implement Redis caching for frequently accessed data
   - Add response caching headers

## Security Considerations

1. **Environment Variables**
   - Never commit sensitive data to your repository
   - Use Render's environment variable feature
   - Rotate JWT secret keys regularly

2. **Database Security**
   - Use strong passwords for database users
   - Enable SSL connections
   - Regularly backup your database

3. **API Security**
   - Implement rate limiting
   - Add request validation
   - Monitor for suspicious activity

## Maintenance

1. **Regular Updates**
   - Keep your .NET Core version updated
   - Update NuGet packages regularly
   - Monitor for security vulnerabilities

2. **Backup Strategy**
   - Set up automated database backups
   - Test restore procedures regularly

3. **Monitoring**
   - Set up health checks
   - Monitor application performance
   - Set up alerts for critical issues

## Support

- **Render Documentation**: [docs.render.com](https://docs.render.com)
- **Render Support**: Available through the Render dashboard
- **Community**: Check Render's community forums for help

## Cost Estimation

### Free Tier
- Web Service: Free (with limitations)
- PostgreSQL: Free (with limitations)
- Total: $0/month

### Paid Plans
- Web Service: $7/month and up
- PostgreSQL: $7/month and up
- Total: $14/month and up

Choose the plan that best fits your needs and budget.