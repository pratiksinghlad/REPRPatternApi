namespace REPRPatternApi.Models.Responses;

public record ProductResponse(
       int Id,
       string Name,
       string Description,
       decimal Price,
       int Stock);

public record ProductsResponse(IEnumerable<ProductResponse> Products);

public record ErrorResponse(string Message);
