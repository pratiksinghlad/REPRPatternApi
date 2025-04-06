namespace REPRPatternApi.Models.Requests;

public record GetProductRequest(int Id);

public record CreateProductRequest(
    string Name,
    string Description,
    decimal Price,
    int Stock);

public record UpdateProductRequest(
    int Id,
    string Name,
    string Description,
    decimal Price,
    int Stock);
