#!/bin/bash

# Simple integration test script for REPR Pattern API endpoints
BASE_URL="http://localhost:5000"

echo "Starting REPR Pattern API Integration Tests..."

# Test GET all products
echo "Testing GET all products..."
RESPONSE=$(curl -s -w "%{http_code}" "$BASE_URL/api/v1/products")
HTTP_CODE="${RESPONSE: -3}"
BODY="${RESPONSE%???}"

if [[ "$HTTP_CODE" == "200" ]]; then
    echo "✅ GET all products: SUCCESS (HTTP $HTTP_CODE)"
    PRODUCT_COUNT=$(echo "$BODY" | jq '.products | length')
    echo "   Found $PRODUCT_COUNT products"
else
    echo "❌ GET all products: FAILED (HTTP $HTTP_CODE)"
    echo "   Response: $BODY"
fi

# Test GET product by ID
echo "Testing GET product by ID..."
RESPONSE=$(curl -s -w "%{http_code}" "$BASE_URL/api/v1/products/1")
HTTP_CODE="${RESPONSE: -3}"
BODY="${RESPONSE%???}"

if [[ "$HTTP_CODE" == "200" ]]; then
    echo "✅ GET product by ID: SUCCESS (HTTP $HTTP_CODE)"
    PRODUCT_NAME=$(echo "$BODY" | jq -r '.name')
    echo "   Product name: $PRODUCT_NAME"
else
    echo "❌ GET product by ID: FAILED (HTTP $HTTP_CODE)"
    echo "   Response: $BODY"
fi

# Test POST create product
echo "Testing POST create product..."
RESPONSE=$(curl -s -w "%{http_code}" -X POST "$BASE_URL/api/v1/products" \
  -H "Content-Type: application/json" \
  -d '{"name":"Test Product","description":"Integration Test Product","price":29.99,"stock":5}')
HTTP_CODE="${RESPONSE: -3}"
BODY="${RESPONSE%???}"

if [[ "$HTTP_CODE" == "201" ]]; then
    echo "✅ POST create product: SUCCESS (HTTP $HTTP_CODE)"
    NEW_PRODUCT_ID=$(echo "$BODY" | jq -r '.id')
    echo "   Created product ID: $NEW_PRODUCT_ID"
else
    echo "❌ POST create product: FAILED (HTTP $HTTP_CODE)"
    echo "   Response: $BODY"
fi

# Test PUT update product
if [[ -n "$NEW_PRODUCT_ID" && "$NEW_PRODUCT_ID" != "null" ]]; then
    echo "Testing PUT update product..."
    RESPONSE=$(curl -s -w "%{http_code}" -X PUT "$BASE_URL/api/v1/products/$NEW_PRODUCT_ID" \
      -H "Content-Type: application/json" \
      -d "{\"id\":$NEW_PRODUCT_ID,\"name\":\"Updated Test Product\",\"description\":\"Updated Integration Test Product\",\"price\":39.99,\"stock\":10}")
    HTTP_CODE="${RESPONSE: -3}"
    BODY="${RESPONSE%???}"

    if [[ "$HTTP_CODE" == "200" ]]; then
        echo "✅ PUT update product: SUCCESS (HTTP $HTTP_CODE)"
        UPDATED_NAME=$(echo "$BODY" | jq -r '.name')
        echo "   Updated product name: $UPDATED_NAME"
    else
        echo "❌ PUT update product: FAILED (HTTP $HTTP_CODE)"
        echo "   Response: $BODY"
    fi

    # Test DELETE product
    echo "Testing DELETE product..."
    RESPONSE=$(curl -s -w "%{http_code}" -X DELETE "$BASE_URL/api/v1/products/$NEW_PRODUCT_ID")
    HTTP_CODE="${RESPONSE: -3}"

    if [[ "$HTTP_CODE" == "204" ]]; then
        echo "✅ DELETE product: SUCCESS (HTTP $HTTP_CODE)"
    else
        echo "❌ DELETE product: FAILED (HTTP $HTTP_CODE)"
    fi
fi

# Test health check
echo "Testing health check..."
RESPONSE=$(curl -s -w "%{http_code}" "$BASE_URL/health/ready")
HTTP_CODE="${RESPONSE: -3}"
BODY="${RESPONSE%???}"

if [[ "$HTTP_CODE" == "200" ]]; then
    echo "✅ Health check: SUCCESS (HTTP $HTTP_CODE)"
    echo "   Status: $BODY"
else
    echo "❌ Health check: FAILED (HTTP $HTTP_CODE)"
    echo "   Response: $BODY"
fi

echo "Integration tests completed!"