using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using REPRPatternApi.Extensions;
using REPRPatternApi.Models.Requests;
using REPRPatternApi.Models.Responses;
using REPRPatternApi.Services;

namespace REPRPatternApi.Endpoints.Products;

/// <summary>
/// Endpoint to create a new product
/// </summary>
public sealed class CreateProductEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("products", HandleAsync)
            .WithName("CreateProduct")
            .WithSummary("Create a new product")
            .WithDescription("Creates a new product with the provided information")
            .WithTags("Products")
            .Produces<ProductResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    /// <summary>
    /// Handles the request to create a new product
    /// </summary>
    /// <param name="request">Create product request</param>
    /// <param name="productService">Product service for data access</param>
    /// <param name="linkGenerator">Link generator for location header</param>
    /// <param name="httpContext">HTTP context</param>
    /// <param name="logger">Logger for structured logging</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created product response</returns>
    private static async Task<IResult> HandleAsync(
        [FromBody] CreateProductRequest request,
        IProductService productService,
        LinkGenerator linkGenerator,
        HttpContext httpContext,
        ILogger<CreateProductEndpoint> logger,
        CancellationToken cancellationToken = default)
    {
        using var activity = logger.BeginScope(new Dictionary<string, object>
        {
            ["Operation"] = "CreateProduct",
            ["ProductName"] = request.Name,
            ["CorrelationId"] = Guid.NewGuid()
        });

        logger.LogInformation("Creating new product: {ProductName}", request.Name);

        // Validate the request
        var validationResult = ValidationExtensions.ValidateRequest(request);
        if (validationResult != null)
        {
            logger.LogWarning("Validation failed for create product request");
            return validationResult;
        }

        try
        {
            var product = await productService.CreateProductAsync(request);

            var locationUri = linkGenerator.GetUriByName(
                httpContext,
                "GetProductById",
                new { id = product.Id }) ?? $"/api/v1/products/{product.Id}";

            logger.LogInformation("Successfully created product with ID {ProductId}", product.Id);

            return Results.Created(locationUri, product);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while creating product: {ProductName}", request.Name);
            return Results.Problem(
                title: "Internal Server Error",
                detail: "An error occurred while creating the product",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}