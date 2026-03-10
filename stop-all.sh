#!/bin/bash
# Bash script to stop all microservices

echo "🛑 Stopping all microservices..."
echo ""

# Stop docker-compose services
echo "Stopping Docker Compose services..."
docker-compose down

if [ $? -eq 0 ]; then
    echo "✅ All microservices stopped"
else
    echo "⚠️  Some services may not have stopped correctly"
fi

echo ""
echo "💡 Note: Your SQL Server and RabbitMQ containers are still running"
echo "   To stop them manually:"
echo "   docker stop sqlserver messaging-qnbztnvt"
echo ""
