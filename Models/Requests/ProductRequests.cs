using System.ComponentModel.DataAnnotations;

namespace REPRPatternApi.Models.Requests;

public record GetProductRequest([Required] int Id);

public record CreateProductRequest(
    [Required, StringLength(100, MinimumLength = 1)] string Name,
    [Required, StringLength(500)] string Description,
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")] decimal Price,
    [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")] int Stock);

public record UpdateProductRequest(
    [Required] int Id,
    [Required, StringLength(100, MinimumLength = 1)] string Name,
    [Required, StringLength(500)] string Description,
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")] decimal Price,
    [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")] int Stock);
