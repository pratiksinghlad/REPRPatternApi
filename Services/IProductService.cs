using REPRPatternApi.Models.Requests;
using REPRPatternApi.Models.Responses;

namespace REPRPatternApi.Services;

public interface IProductService
{
    Task<IEnumerable<ProductResponse>> GetAllProductsAsync();
    Task<ProductResponse?> GetProductByIdAsync(int id);
    Task<ProductResponse> CreateProductAsync(CreateProductRequest request);
    Task<ProductResponse?> UpdateProductAsync(UpdateProductRequest request);
    Task<bool> DeleteProductAsync(int id);
}
