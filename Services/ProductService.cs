using REPRPatternApi.Data.Entities;
using REPRPatternApi.Models.Requests;
using REPRPatternApi.Models.Responses;

namespace REPRPatternApi.Services;

public class ProductService : IProductService
{
    private readonly List<Product> _products;
    private int _nextId = 1;

    public ProductService()
    {
        // Initialize with sample data
        _products = new List<Product>
            {
                new Product { Id = _nextId++, Name = "Laptop", Description = "Gaming Laptop", Price = 1299.99m, Stock = 25 },
                new Product { Id = _nextId++, Name = "Phone", Description = "Smartphone", Price = 899.99m, Stock = 50 },
                new Product { Id = _nextId++, Name = "Headphones", Description = "Wireless Headphones", Price = 199.99m, Stock = 100 }
            };
    }

    public Task<IEnumerable<ProductResponse>> GetAllProductsAsync()
    {
        var productResponses = _products.Select(p =>
            new ProductResponse(p.Id, p.Name, p.Description, p.Price, p.Stock));

        return Task.FromResult(productResponses);
    }

    public Task<ProductResponse?> GetProductByIdAsync(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);

        if (product == null)
            return Task.FromResult<ProductResponse?>(null);

        return Task.FromResult<ProductResponse?>(
            new ProductResponse(
                product.Id,
                product.Name,
                product.Description,
                product.Price,
                product.Stock));
    }

    public Task<ProductResponse> CreateProductAsync(CreateProductRequest request)
    {
        var product = new Product
        {
            Id = _nextId++,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock
        };

        _products.Add(product);

        return Task.FromResult(
            new ProductResponse(
                product.Id,
                product.Name,
                product.Description,
                product.Price,
                product.Stock));
    }

    public Task<ProductResponse?> UpdateProductAsync(UpdateProductRequest request)
    {
        var product = _products.FirstOrDefault(p => p.Id == request.Id);

        if (product == null)
            return Task.FromResult<ProductResponse?>(null);

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.Stock = request.Stock;

        return Task.FromResult<ProductResponse?>(
            new ProductResponse(
                product.Id,
                product.Name,
                product.Description,
                product.Price,
                product.Stock));
    }

    public Task<bool> DeleteProductAsync(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);

        if (product == null)
            return Task.FromResult(false);

        _products.Remove(product);
        return Task.FromResult(true);
    }
}
