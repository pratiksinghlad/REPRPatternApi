# MongoDB CRUD Operations with REPR Pattern

## Overview
This implementation demonstrates a complete CRUD operations setup using MongoDB with the embedding (denormalization) pattern following the REPR (Request-Endpoint-Response-Repository) pattern architecture.

## Architecture Components

### 1. Data Layer
- **Entities**: `BlogPost` and `Comment` classes with MongoDB BSON attributes
- **Repository**: `BlogPostRepository` with async CRUD operations
- **Connection**: MongoDB.Driver with proper DI registration

### 2. Service Layer  
- **Interface**: `IBlogPostService` defining business operations
- **Implementation**: `BlogPostService` with logging and mapping logic

### 3. API Layer (REPR Pattern)
- **Endpoints**: Separate endpoint classes for each operation:
  - `CreateBlogPostEndpoint` - POST /blogposts
  - `GetAllBlogPostsEndpoint` - GET /blogposts  
  - `GetBlogPostByIdEndpoint` - GET /blogposts/{id}
  - `UpdateBlogPostEndpoint` - PUT /blogposts/{id}
  - `DeleteBlogPostEndpoint` - DELETE /blogposts/{id}

### 4. Models
- **Requests**: `BlogPostCreateRequest`, `BlogPostUpdateRequest` with validation
- **Responses**: `BlogPostResponse`, `BlogPostsResponse` DTOs

## MongoDB Embedding Pattern

The `BlogPost` document embeds `Comments` directly, demonstrating denormalization:

```csharp
public class BlogPost
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public List<Comment> Comments { get; set; } // Embedded comments
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

## Configuration

### Connection String
```json
{
  "ConnectionStrings": {
    "MongoDb": "mongodb://localhost:27017/REPRPatternDb"
  }
}
```

### Dependency Injection
Registered in `Program.cs`:
```csharp
builder.Services.AddMongoDb(builder.Configuration);
```

## Features Implemented

✅ **CRUD Operations**: Create, Read, Update, Delete  
✅ **Embedding Pattern**: Comments embedded in BlogPost documents  
✅ **Validation**: Request validation with data annotations  
✅ **Error Handling**: Proper HTTP status codes and error responses  
✅ **Logging**: Structured logging with correlation IDs  
✅ **Async/Await**: All database operations are async  
✅ **Service Layer**: Business logic separation  
✅ **Repository Pattern**: Data access abstraction  
✅ **REPR Pattern**: Endpoint-based architecture  
✅ **OpenAPI Documentation**: Swagger/Scalar documentation  

## Testing

Use the `BlogPosts.http` file to test all endpoints:
1. Create a blog post with embedded comments
2. Retrieve all blog posts  
3. Get specific blog post by ID
4. Update blog post with new comments
5. Delete blog post

## Best Practices Followed

- **Microsoft Guidelines**: Async/await, proper DI, validation
- **MongoDB Best Practices**: Proper indexing potential, document size management
- **Clean Architecture**: Separation of concerns, dependency inversion
- **Error Handling**: Graceful exception handling with logging
- **Security**: Input validation, safe connection string management

## Next Steps

1. Add MongoDB indexes for frequently queried fields
2. Implement pagination for GetAll endpoint
3. Add comment-specific operations (add/update/delete individual comments)
4. Add authentication and authorization
5. Implement caching strategies
6. Add integration tests
