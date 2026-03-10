#!/bin/bash
# Bash script to start all microservices (for WSL/Linux users)

echo "🚀 Starting Practical Microservices..."
echo ""

# Step 1: Start existing infrastructure containers
echo "📦 Starting existing infrastructure containers..."
if docker start sqlserver 2>/dev/null; then
    echo "   ✅ SQL Server started"
else
    echo "   ⚠️  SQL Server container not found or already running"
fi

if docker start messaging-qnbztnvt 2>/dev/null; then
    echo "   ✅ RabbitMQ started"
else
    echo "   ⚠️  RabbitMQ container not found or already running"
fi

echo ""
echo "⏳ Waiting 5 seconds for infrastructure to be ready..."
sleep 5

# Step 2: Build and start microservices with docker-compose
echo ""
echo "🏗️  Building and starting microservices..."
docker-compose up --build -d

if [ $? -eq 0 ]; then
    echo ""
    echo "✅ All services started successfully!"
    echo ""
    echo "📋 Service URLs:"
    echo "   UserService:          http://localhost:5001/swagger"
    echo "   ProductService:       http://localhost:5002/swagger"
    echo "   OrderService:         http://localhost:5003/swagger"
    echo "   PaymentService:       http://localhost:5004/swagger"
    echo "   NotificationService:  http://localhost:5005/swagger"
    echo "   OrchestratorService:  http://localhost:5006/swagger"
    echo ""
    echo "   RabbitMQ Management:  http://localhost:15672"
    echo "   (username: guest, password: guest)"
    echo ""
    echo "💡 Tips:"
    echo "   - View logs: docker-compose logs -f [service-name]"
    echo "   - Stop all:  docker-compose down"
    echo "   - Or use:    ./stop-all.sh"
else
    echo ""
    echo "❌ Failed to start services. Check errors above."
    exit 1
fi
