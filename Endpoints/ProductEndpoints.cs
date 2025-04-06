using REPRPatternApi.Models.Requests;
using REPRPatternApi.Models.Responses;
using REPRPatternApi.Services;

namespace REPRPatternApi.Endpoints;

public class ProductEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("products");

        // GET all products
        group.MapGet("/", GetAllProductsAsync)
            .WithName("GetAllProducts")
            .Produces<ProductsResponse>(StatusCodes.Status200OK);

        // GET product by id
        group.MapGet("/{id}", GetProductByIdAsync)
            .WithName("GetProductById")
            .Produces<ProductResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);

        // POST create product
        group.MapPost("/", CreateProductAsync)
            .WithName("CreateProduct")
            .Produces<ProductResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        // PUT update product
        group.MapPut("/{id}", UpdateProductAsync)
            .WithName("UpdateProduct")
            .Produces<ProductResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
            .ProducesValidationProblem();

        // DELETE product
        group.MapDelete("/{id}", DeleteProductAsync)
            .WithName("DeleteProduct")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound);
    }

    private async Task<IResult> GetAllProductsAsync(IProductService productService)
    {
        var products = await productService.GetAllProductsAsync();
        return Results.Ok(new ProductsResponse(products));
    }

    private async Task<IResult> GetProductByIdAsync(int id, IProductService productService)
    {
        var product = await productService.GetProductByIdAsync(id);
        return product == null
            ? Results.NotFound(new ErrorResponse($"Product with ID {id} not found"))
            : Results.Ok(product);
    }

    private async Task<IResult> CreateProductAsync(
        CreateProductRequest request,
        IProductService productService,
        LinkGenerator linkGenerator,
        HttpContext httpContext)
    {
        // Validate request
        if (string.IsNullOrWhiteSpace(request.Name))
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                { "Name", new[] { "Product name is required" } }
            });

        if (request.Price <= 0)
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                { "Price", new[] { "Price must be greater than zero" } }
            });

        var product = await productService.CreateProductAsync(request);

        var locationUri = linkGenerator.GetUriByName(
            httpContext,
            "GetProductById",
            new { id = product.Id }) ?? $"/api/{product.Id}";

        return Results.Created(locationUri, product);
    }

    private async Task<IResult> UpdateProductAsync(
        int id,
        UpdateProductRequest request,
        IProductService productService)
    {
        if (id != request.Id)
            return Results.BadRequest(new ErrorResponse("ID in route must match ID in request body"));

        // Validate request
        if (string.IsNullOrWhiteSpace(request.Name))
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                { "Name", new[] { "Product name is required" } }
            });

        if (request.Price <= 0)
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                { "Price", new[] { "Price must be greater than zero" } }
            });

        var product = await productService.UpdateProductAsync(request);
        return product == null
            ? Results.NotFound(new ErrorResponse($"Product with ID {id} not found"))
            : Results.Ok(product);
    }

    private async Task<IResult> DeleteProductAsync(int id, IProductService productService)
    {
        var result = await productService.DeleteProductAsync(id);
        return result
            ? Results.NoContent()
            : Results.NotFound(new ErrorResponse($"Product with ID {id} not found"));
    }
}
