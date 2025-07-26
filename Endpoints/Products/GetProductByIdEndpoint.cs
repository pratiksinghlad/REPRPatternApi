using REPRPatternApi.Models.Responses;
using REPRPatternApi.Services;

namespace REPRPatternApi.Endpoints.Products;

/// <summary>
/// Endpoint to retrieve a product by its ID
/// </summary>
public sealed class GetProductByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("products/{id:int}", HandleAsync)
            .WithName("GetProductById")
            .WithSummary("Get product by ID")
            .WithDescription("Retrieves a specific product by its unique identifier")
            .WithTags("Products")
            .Produces<ProductResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    /// <summary>
    /// Handles the request to get a product by ID
    /// </summary>
    /// <param name="id">The product ID</param>
    /// <param name="productService">Product service for data access</param>
    /// <param name="logger">Logger for structured logging</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Product response or not found</returns>
    private static async Task<IResult> HandleAsync(
        int id,
        IProductService productService,
        ILogger<GetProductByIdEndpoint> logger,
        CancellationToken cancellationToken = default)
    {
        using var activity = logger.BeginScope(new Dictionary<string, object>
        {
            ["Operation"] = "GetProductById",
            ["ProductId"] = id,
            ["CorrelationId"] = Guid.NewGuid()
        });

        logger.LogInformation("Retrieving product with ID {ProductId}", id);

        if (id <= 0)
        {
            logger.LogWarning("Invalid product ID provided: {ProductId}", id);
            return Results.BadRequest(new ErrorResponse("Product ID must be greater than 0"));
        }

        try
        {
            var product = await productService.GetProductByIdAsync(id);
            
            if (product == null)
            {
                logger.LogWarning("Product with ID {ProductId} not found", id);
                return Results.NotFound(new ErrorResponse($"Product with ID {id} not found"));
            }

            logger.LogInformation("Successfully retrieved product with ID {ProductId}", id);
            return Results.Ok(product);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while retrieving product with ID {ProductId}", id);
            return Results.Problem(
                title: "Internal Server Error",
                detail: $"An error occurred while retrieving product with ID {id}",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}