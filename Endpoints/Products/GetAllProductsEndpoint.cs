using REPRPatternApi.Models.Responses;
using REPRPatternApi.Services;

namespace REPRPatternApi.Endpoints.Products;

/// <summary>
/// Endpoint to retrieve all products
/// </summary>
public sealed class GetAllProductsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("products", HandleAsync)
            .WithName("GetAllProducts")
            .WithSummary("Get all products")
            .WithDescription("Retrieves a list of all available products")
            .WithTags("Products")
            .Produces<ProductsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    /// <summary>
    /// Handles the request to get all products
    /// </summary>
    /// <param name="productService">Product service for data access</param>
    /// <param name="logger">Logger for structured logging</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>All products response</returns>
    private static async Task<IResult> HandleAsync(
        IProductService productService,
        ILogger<GetAllProductsEndpoint> logger,
        CancellationToken cancellationToken = default)
    {
        using var activity = logger.BeginScope(new Dictionary<string, object>
        {
            ["Operation"] = "GetAllProducts",
            ["CorrelationId"] = Guid.NewGuid()
        });

        logger.LogInformation("Retrieving all products");

        try
        {
            var products = await productService.GetAllProductsAsync();
            var response = new ProductsResponse(products);

            logger.LogInformation("Successfully retrieved {ProductCount} products", response.Products.Count());
            
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while retrieving all products");
            return Results.Problem(
                title: "Internal Server Error",
                detail: "An error occurred while retrieving products",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}