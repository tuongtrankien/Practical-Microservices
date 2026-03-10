# Docker Setup Guide - Microservices

## 🎯 Overview

This project uses **Option 2: Partial Docker Compose** approach:
- ✅ Reuses your **existing RabbitMQ and SQL Server** containers
- ✅ Orchestrates all **6 microservices** with Docker Compose
- ✅ One command to start everything: `.\start-all.ps1`
- ✅ Works on **Windows with Docker Desktop** (no WSL required)

---

## 📋 Prerequisites

### Required:
- [Docker Desktop for Windows](https://www.docker.com/products/docker-desktop/) (installed ✅)
- Existing containers running:
  - `sqlserver` - SQL Server 2022
  - `messaging-qnbztnvt` - RabbitMQ with management plugin

### Verify Prerequisites:
```powershell
# Check Docker is running
docker --version

# Check existing containers
docker ps -a
```

---

## 🚀 Quick Start

### 1. Start Everything (ONE COMMAND!)
```powershell
.\start-all.ps1
```

This script will:
1. ✅ Start your existing SQL Server container
2. ✅ Start your existing RabbitMQ container  
3. ✅ Build Docker images for all 6 microservices
4. ✅ Start all microservices in containers
5. ✅ Configure networking between services

### 2. Access Services

| Service | Swagger UI | Port |
|---------|-----------|------|
| UserService | http://localhost:5001/swagger | 5001 |
| ProductService | http://localhost:5002/swagger | 5002 |
| OrderService | http://localhost:5003/swagger | 5003 |
| PaymentService | http://localhost:5004/swagger | 5004 |
| NotificationService | http://localhost:5005/swagger | 5005 |
| OrchestratorService | http://localhost:5006/swagger | 5006 |
| RabbitMQ Management | http://localhost:15672 | 15672 |

### 3. View Logs
```powershell
# All services
.\logs.ps1

# Specific service
.\logs.ps1 userservice
.\logs.ps1 orderservice
```

### 4. Stop Everything
```powershell
.\stop-all.ps1
```

---

## 🔧 Manual Commands

If you prefer manual control:

### Build all services
```powershell
docker-compose build
```

### Start all services
```powershell
docker-compose up -d
```

### Stop all services
```powershell
docker-compose down
```

### Rebuild and restart a specific service
```powershell
docker-compose up -d --build userservice
```

### View running containers
```powershell
docker-compose ps
```

### Follow logs for a service
```powershell
docker-compose logs -f orderservice
```

---

## 🏗️ Architecture Details

### Network Configuration

All microservices run in a Docker bridge network called `microservices-network`.

Services communicate with **host containers** (SQL Server, RabbitMQ) using:
```yaml
extra_hosts:
  - "host.docker.internal:host-gateway"
```

This allows containers to access your existing infrastructure at:
- SQL Server: `host.docker.internal:1433`
- RabbitMQ: `host.docker.internal:5672`

### Port Mapping

| Container Port | Host Port | Service |
|---------------|-----------|---------|
| 80 | 5001 | UserService |
| 80 | 5002 | ProductService |
| 80 | 5003 | OrderService |
| 80 | 5004 | PaymentService |
| 80 | 5005 | NotificationService |
| 80 | 5006 | OrchestratorService |

### Environment Variables

Each service is configured with:
- `ASPNETCORE_ENVIRONMENT=Development`
- `ConnectionStrings__DefaultConnection` (points to host SQL Server)
- `RabbitMQ__Host=host.docker.internal`

---

## 🔄 Development Workflow

### Making Code Changes

1. **Edit your code** in Visual Studio Code or Visual Studio
2. **Rebuild the service**:
   ```powershell
   docker-compose up -d --build userservice
   ```
3. **View logs** to verify changes:
   ```powershell
   docker-compose logs -f userservice
   ```

### Hot Reload (Optional)

For development with hot reload, you can:
1. Stop a service from Docker: `docker-compose stop userservice`
2. Run it locally: `cd UserService/UserService.API; dotnet run`
3. Other services in Docker will still work

### Database Migrations

Run migrations **before** starting services:

```powershell
# UserService example
cd UserService
dotnet ef database update --project UserService.Infrastructure --startup-project UserService.API

# ProductService example  
cd ProductService
dotnet ef database update --project ProductService.Infrastructure --startup-project ProductService.API
```

---

## 🐳 When to Use WSL Ubuntu Instead of Windows?

### ✅ Use Windows (Current Setup) When:
1. **You're developing on Windows** ✅ (Your case)
2. **You have Docker Desktop installed** ✅
3. **Performance is acceptable** for your workflow
4. **You prefer PowerShell/CMD** scripting
5. **Your team uses Windows**

### ⚠️ Use WSL Ubuntu When:
1. **Linux-specific dependencies**: Your microservices need Linux-only packages
2. **Performance issues**: Docker on Windows can be slower for some workloads
3. **Production parity**: Your production runs on Linux servers
4. **File system performance**: Large projects with many files (node_modules, etc.)
5. **Bash scripting preference**: Team prefers bash over PowerShell
6. **CI/CD alignment**: Your CI/CD runs on Linux

### 🔄 Switching to WSL (If Needed)

If you decide to use WSL later:

1. **Install WSL 2**:
   ```powershell
   wsl --install -d Ubuntu
   ```

2. **Configure Docker Desktop** to use WSL 2 backend

3. **Move project to WSL**:
   ```bash
   # In WSL terminal
   cd ~
   git clone <your-repo> Practical-Microservices
   cd Practical-Microservices
   ```

4. **Use bash scripts** instead:
   ```bash
   ./start-all.sh  # Create this for bash
   ```

### 💡 Recommendation for This Project:

**Stay on Windows** because:
- ✅ You already have infrastructure setup
- ✅ .NET runs great on Windows
- ✅ No Linux-specific dependencies
- ✅ Simpler setup (no WSL complexity)

**Consider WSL if**:
- ❌ You face performance issues
- ❌ You need Linux-only tools
- ❌ Your production is Linux-based

---

## 🆘 Troubleshooting

### Services can't connect to SQL Server

**Problem**: `A network-related or instance-specific error`

**Solution**:
1. Ensure SQL Server container is running: `docker ps | grep sqlserver`
2. Verify port 1433 is accessible: `Test-NetConnection localhost -Port 1433`
3. Check connection string in [docker-compose.yml](docker-compose.yml)

### Services can't connect to RabbitMQ

**Problem**: `Connection refused to localhost:5672`

**Solution**:
1. Start RabbitMQ: `docker start messaging-qnbztnvt`
2. Verify it's running: `docker ps | grep rabbit`
3. Check management UI: http://localhost:15672

### Build fails with "project reference not found"

**Problem**: Dockerfile can't find referenced projects

**Solution**:
Check that your `.dockerignore` doesn't exclude necessary files:
```bash
cat .dockerignore
```

### Port already in use

**Problem**: `Bind for 0.0.0.0:5001 failed: port is already allocated`

**Solution**:
1. Find what's using the port:
   ```powershell
   netstat -ano | findstr :5001
   ```
2. Stop the conflicting process or change port in [docker-compose.yml](docker-compose.yml)

### Services start but show errors

**Problem**: Services crash or restart constantly

**Solution**:
1. Check logs:
   ```powershell
   docker-compose logs userservice
   ```
2. Common causes:
   - Database not migrated (run `dotnet ef database update`)
   - Wrong connection string
   - Missing environment variables

### Docker Desktop not starting

**Problem**: Docker Desktop hangs or fails to start

**Solution**:
1. Restart Docker Desktop
2. Check Windows Subsystem for Linux is enabled
3. Ensure Hyper-V is enabled (or WSL 2)
4. Check Docker Desktop Settings → Resources

---

## 📊 Resource Usage

Typical resource usage with all services running:

| Component | CPU | Memory |
|-----------|-----|--------|
| SQL Server | ~5% | ~500 MB |
| RabbitMQ | ~2% | ~200 MB |
| Each Microservice | ~1-3% | ~100-150 MB |
| **Total** | **~20%** | **~1.5-2 GB** |

---

## 🎓 Docker Compose Commands Cheat Sheet

```powershell
# Build without cache
docker-compose build --no-cache

# Start in foreground (see logs directly)
docker-compose up

# Start in background
docker-compose up -d

# Stop services (keeps containers)
docker-compose stop

# Remove containers (keeps images)
docker-compose down

# Remove containers and volumes
docker-compose down -v

# Remove containers, volumes, and images
docker-compose down -v --rmi all

# Scale a service (if stateless)
docker-compose up -d --scale notificationservice=3

# Execute command in running container
docker-compose exec userservice bash

# Restart a service
docker-compose restart userservice
```

---

## 🚀 Production Considerations

When deploying to production:

1. **Use proper secrets management** (not hardcoded passwords)
2. **Configure health checks** in docker-compose.yml
3. **Add resource limits**:
   ```yaml
   deploy:
     resources:
       limits:
         cpus: '0.5'
         memory: 512M
   ```
4. **Use production databases** (not host containers)
5. **Implement proper logging** (ELK, Seq, Application Insights)
6. **Add reverse proxy** (Nginx, Traefik)
7. **Enable HTTPS** with certificates
8. **Use orchestration** (Kubernetes, Docker Swarm)

---

## 📚 Additional Resources

- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [.NET Docker Images](https://hub.docker.com/_/microsoft-dotnet)
- [Multi-stage Builds](https://docs.docker.com/build/building/multi-stage/)
- [Docker Networking](https://docs.docker.com/network/)

---

## 🆘 Getting Help

If you encounter issues:

1. Check logs: `.\logs.ps1 [service-name]`
2. Verify containers: `docker ps -a`
3. Check Docker Desktop dashboard
4. Review [QUICK_START.md](QUICK_START.md)
5. See [TROUBLESHOOTING.md] (if exists)

---

## 📝 Notes

- **Current setup**: Uses existing RabbitMQ and SQL Server on host
- **Alternative**: Full Docker Compose with dedicated containers for everything
- **Windows vs WSL**: This setup works great on Windows, switch to WSL only if needed
- **Development**: You can mix Docker services with local `dotnet run` for debugging
