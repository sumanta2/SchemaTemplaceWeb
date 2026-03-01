# SchemaTemplaceWeb

## Environment Variables & Configuration

This application uses ASP.NET Core's configuration system to manage environment-specific settings. Connection strings and other sensitive data should be passed via environment variables, not hardcoded in `appsettings.json`.

### How It Works

ASP.NET Core automatically reads environment variables that follow the naming convention:

- Use double underscores `__` to represent nested JSON hierarchy

- For `ConnectionStrings.DefaultConnection`, the environment variable is: `ConnectionStrings__DefaultConnection`

### Setting Environment Variables - Different Platforms

#### 1. **Local Development (Windows CMD)**

```cmd
set ConnectionStrings__DefaultConnection=Server=DESKTOP-RRET3GI\SQLEXPRESS;Database=DMS;Trusted_Connection=True;TrustServerCertificate=True;
dotnet run
```

#### 2. **Local Development (Windows PowerShell)**

```powershell
$env:ConnectionStrings__DefaultConnection="Server=DESKTOP-RRET3GI\SQLEXPRESS;Database=DMS;Trusted_Connection=True;TrustServerCertificate=True;"
dotnet run
```

#### 3. **Local Development (macOS/Linux)**

```bash
export ConnectionStrings__DefaultConnection="Server=localhost;Database=DMS;User Id=sa;Password=YourPassword;"
dotnet run
```

#### 4. **Docker Container**

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY . .
ENV ConnectionStrings__DefaultConnection="Server=db-server;Database=DMS;User Id=sa;Password=YourPassword;"
EXPOSE 5000
CMD ["dotnet", "SchemaTemplaceWeb.dll"]
```

Or in `docker-compose.yml`:

```yaml
services:
  web:
    build: .
    environment:
      - ConnectionStrings__DefaultConnection=Server=db;Database=DMS;User Id=sa;Password=YourPassword;
    ports:
      - "5000:5000"
```
