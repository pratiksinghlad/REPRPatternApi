using REPRPatternApi.Models.Responses;
using REPRPatternApi.Services;

namespace REPRPatternApi.Endpoints.Products;

/// <summary>
/// Endpoint to delete a product by its ID
/// </summary>
public sealed class DeleteProductEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("products/{id:int}", HandleAsync)
            .WithName("DeleteProduct")
            .WithSummary("Delete a product")
            .WithDescription("Deletes a product by its unique identifier")
            .WithTags("Products")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    /// <summary>
    /// Handles the request to delete a product by ID
    /// </summary>
    /// <param name="id">The product ID</param>
    /// <param name="productService">Product service for data access</param>
    /// <param name="logger">Logger for structured logging</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content if successful, not found if product doesn't exist</returns>
    private static async Task<IResult> HandleAsync(
        int id,
        IProductService productService,
        ILogger<DeleteProductEndpoint> logger,
        CancellationToken cancellationToken = default)
    {
        using var activity = logger.BeginScope(new Dictionary<string, object>
        {
            ["Operation"] = "DeleteProduct",
            ["ProductId"] = id,
            ["CorrelationId"] = Guid.NewGuid()
        });

        logger.LogInformation("Deleting product with ID {ProductId}", id);

        if (id <= 0)
        {
            logger.LogWarning("Invalid product ID provided: {ProductId}", id);
            return Results.BadRequest(new ErrorResponse("Product ID must be greater than 0"));
        }

        try
        {
            var result = await productService.DeleteProductAsync(id);
            
            if (!result)
            {
                logger.LogWarning("Product with ID {ProductId} not found for deletion", id);
                return Results.NotFound(new ErrorResponse($"Product with ID {id} not found"));
            }

            logger.LogInformation("Successfully deleted product with ID {ProductId}", id);
            return Results.NoContent();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while deleting product with ID {ProductId}", id);
            return Results.Problem(
                title: "Internal Server Error",
                detail: $"An error occurred while deleting product with ID {id}",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}