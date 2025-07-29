# REPR Pattern API Implementation

This project demonstrates a complete implementation of the **REPR (Request-Endpoint-Response) pattern** using .NET 8 minimal APIs, following Microsoft's best practices and modern architectural patterns.

## 🏗️ Architecture Overview

The REPR pattern organizes API endpoints as self-contained units with three key components:

- **Request**: Input models with validation
- **Endpoint**: Single-purpose handlers with business logic  
- **Response**: Typed output models

## 📁 Project Structure

```
REPRPatternApi/
├── Endpoints/
│   ├── Products/
│   │   ├── GetAllProductsEndpoint.cs
│   │   ├── GetProductByIdEndpoint.cs
│   │   ├── CreateProductEndpoint.cs
│   │   ├── UpdateProductEndpoint.cs
│   │   └── DeleteProductEndpoint.cs
│   └── External/
│       └── GetExternalDataEndpoint.cs
├── Models/
│   ├── Requests/
│   │   └── ProductRequests.cs
│   └── Responses/
│       ├── ProductResponses.cs
│       └── External/
│           └── ExternalDataResponses.cs
├── Services/
│   ├── ProductService.cs
│   └── ExternalApiService.cs
├── Extensions/
│   ├── EndpointExtensions.cs
│   └── ValidationExtensions.cs
└── Tests/
    └── integration_test.sh
```

## ✨ Key Features

### 🎯 Single Responsibility Endpoints
Each endpoint is a separate class with a single purpose:
- `GetAllProductsEndpoint` - Retrieve all products
- `GetProductByIdEndpoint` - Retrieve a specific product
- `CreateProductEndpoint` - Create new products
- `UpdateProductEndpoint` - Update existing products
- `DeleteProductEndpoint` - Delete products

### 🛡️ Comprehensive Validation
- **DataAnnotations** validation on request models
- **Centralized validation** infrastructure
- **Consistent error responses** across all endpoints

### 📊 Structured Logging
- **Correlation IDs** for request tracing
- **Structured logging** with proper scoping
- **Comprehensive error logging** with context

### 🔧 Modern .NET Features
- **Record types** for immutable request/response models
- **Minimal APIs** for performance and simplicity
- **Dependency injection** optimization
- **Source generation** for JSON serialization
- **Nullable reference types** enforcement

### 📈 Performance & Scalability
- **Response compression** (Brotli, Gzip)
- **Response caching** with memory and distributed cache
- **HTTP/2 support** and security headers
- **Health checks** for monitoring

### 📚 Documentation
- **Comprehensive XML documentation** for all endpoints
- **OpenAPI/Swagger** integration
- **Scalar documentation** UI with environment-specific theming

## 🚀 API Endpoints

### Products API
- `GET /api/v1/products` - Get all products
- `GET /api/v1/products/{id}` - Get product by ID
- `POST /api/v1/products` - Create new product
- `PUT /api/v1/products/{id}` - Update existing product
- `DELETE /api/v1/products/{id}` - Delete product

### External Data API
- `GET /api/v1/external/data` - Get external API data

### Health & Monitoring
- `GET /health/ready` - Readiness check
- `GET /health/live` - Liveness check

### Documentation
- `/scalar/v1` - Interactive API documentation

## 🧪 Testing

Run the integration test suite:

```bash
chmod +x Tests/integration_test.sh
./Tests/integration_test.sh
```

## 🛠️ Development

### Prerequisites
- .NET 8.0 SDK or later
- Any IDE supporting .NET (Visual Studio, VS Code, Rider)

### Running the Application
```bash
dotnet run --urls "http://localhost:5000"
```

### Building
```bash
dotnet build
```

### Running Tests
```bash
./Tests/integration_test.sh
```

## 📋 Request/Response Examples

### Create Product Request
```json
{
  "name": "Gaming Laptop",
  "description": "High-performance gaming laptop",
  "price": 1299.99,
  "stock": 25
}
```

### Product Response
```json
{
  "id": 1,
  "name": "Gaming Laptop", 
  "description": "High-performance gaming laptop",
  "price": 1299.99,
  "stock": 25
}
```

### Error Response
```json
{
  "message": "Product with ID 999 not found"
}
```

## 🎯 REPR Pattern Benefits

1. **Single Responsibility**: Each endpoint has one clear purpose
2. **Testability**: Easy to unit test individual endpoints
3. **Maintainability**: Changes are isolated to specific endpoints
4. **Scalability**: Can scale individual endpoints independently
5. **Documentation**: Self-documenting through endpoint structure
6. **Type Safety**: Strong typing throughout request/response flow

## 🔒 Security Features

- **Security headers** (X-Frame-Options, X-Content-Type-Options, etc.)
- **HTTPS redirection** in production
- **CORS policy** configuration
- **Input validation** to prevent common attacks
- **Structured error handling** without information leakage

## 📦 Dependencies

- **ASP.NET Core** - Web framework
- **FluentValidation** - Validation library
- **Polly** - Resilience and transient-fault handling
- **NSwag** - OpenAPI/Swagger tooling
- **Scalar** - API documentation UI
- **Autofac** - Dependency injection container

## 🚀 Production Considerations

- Enable **PublishAot** for AOT compilation
- Configure **distributed caching** (Redis, SQL Server)
- Set up **monitoring** and **observability**
- Implement **rate limiting**
- Add **authentication/authorization**
- Set up **CI/CD pipelines**

## 📈 Performance Optimizations

- **Source generators** for JSON serialization
- **Memory pooling** for object allocation
- **Async/await** patterns throughout
- **Response compression** for reduced bandwidth
- **HTTP/2** support for multiplexing

This implementation serves as a comprehensive example of the REPR pattern following .NET best practices and modern architectural principles.