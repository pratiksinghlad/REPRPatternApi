using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using REPRPatternApi.Extensions;
using REPRPatternApi.Models.Requests;
using REPRPatternApi.Models.Responses;
using REPRPatternApi.Services;

namespace REPRPatternApi.Endpoints.Products;

/// <summary>
/// Endpoint to update an existing product
/// </summary>
public sealed class UpdateProductEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("products/{id:int}", HandleAsync)
            .WithName("UpdateProduct")
            .WithSummary("Update an existing product")
            .WithDescription("Updates an existing product with the provided information")
            .WithTags("Products")
            .Produces<ProductResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    /// <summary>
    /// Handles the request to update an existing product
    /// </summary>
    /// <param name="id">The product ID from route</param>
    /// <param name="request">Update product request</param>
    /// <param name="productService">Product service for data access</param>
    /// <param name="logger">Logger for structured logging</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated product response or not found</returns>
    private static async Task<IResult> HandleAsync(
        int id,
        [FromBody] UpdateProductRequest request,
        IProductService productService,
        ILogger<UpdateProductEndpoint> logger,
        CancellationToken cancellationToken = default)
    {
        using var activity = logger.BeginScope(new Dictionary<string, object>
        {
            ["Operation"] = "UpdateProduct",
            ["ProductId"] = id,
            ["ProductName"] = request.Name,
            ["CorrelationId"] = Guid.NewGuid()
        });

        logger.LogInformation("Updating product with ID {ProductId}", id);

        // Validate route ID matches request ID
        if (id != request.Id)
        {
            logger.LogWarning("Route ID {RouteId} does not match request ID {RequestId}", id, request.Id);
            return Results.BadRequest(new ErrorResponse("ID in route must match ID in request body"));
        }

        if (id <= 0)
        {
            logger.LogWarning("Invalid product ID provided: {ProductId}", id);
            return Results.BadRequest(new ErrorResponse("Product ID must be greater than 0"));
        }

        // Validate the request
        var validationResult = ValidationExtensions.ValidateRequest(request);
        if (validationResult != null)
        {
            logger.LogWarning("Validation failed for update product request");
            return validationResult;
        }

        try
        {
            var product = await productService.UpdateProductAsync(request);
            
            if (product == null)
            {
                logger.LogWarning("Product with ID {ProductId} not found for update", id);
                return Results.NotFound(new ErrorResponse($"Product with ID {id} not found"));
            }

            logger.LogInformation("Successfully updated product with ID {ProductId}", id);
            return Results.Ok(product);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while updating product with ID {ProductId}", id);
            return Results.Problem(
                title: "Internal Server Error",
                detail: $"An error occurred while updating product with ID {id}",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}