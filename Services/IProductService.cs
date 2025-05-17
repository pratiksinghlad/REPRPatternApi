using REPRPatternApi.Models.Requests;
using REPRPatternApi.Models.Responses;

namespace REPRPatternApi.Services;

public interface IProductService
{
    ValueTask<IEnumerable<ProductResponse>> GetAllProductsAsync();
    ValueTask<ProductResponse?> GetProductByIdAsync(int id);
    ValueTask<ProductResponse> CreateProductAsync(CreateProductRequest request);
    ValueTask<ProductResponse?> UpdateProductAsync(UpdateProductRequest request);
    ValueTask<bool> DeleteProductAsync(int id);
}
