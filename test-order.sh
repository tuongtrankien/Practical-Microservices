#!/bin/bash

echo "Creating order..."
curl -X POST http://localhost:5003/api/orders \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "42dccac8-0ec2-47b2-85d8-ed7710af8db2",
    "shippingAddress": "789 Test Lane",
    "orderItems": [
      {
        "productId": "83e9f86e-871d-4ef3-bd1b-646943882508",
        "productName": "Test Product",
        "quantity": 3,
        "unitPrice": 99.99
      }
    ]
  }'

echo ""
echo "==================================="
echo "Waiting 2 seconds for events to propagate..."
sleep 2

echo "==================================="
echo "CHECKING ORCHESTRATOR LOGS:"
echo "==================================="
docker compose logs orchestratorservice --tail=30 | grep -A 2 -B 2 -E "(OrderPlaced|Payment|Stock|SAGA|🎯|📤|✅|❌)"

echo ""
echo "==================================="
echo "CHECKING PAYMENT SERVICE LOGS:"
echo "==================================="
docker compose logs paymentservice --tail=30 | grep -A 2 -B 2 -E "(Payment|Order|💳|✅|❌)"

echo ""
echo "==================================="
echo "CHECKING PRODUCT SERVICE LOGS:"
echo "==================================="
docker compose logs productservice --tail=30 | grep -A 2 -B 2 -E "(Stock|Product|📦|✅|❌)"
