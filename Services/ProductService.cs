using System.Collections.Concurrent;
using REPRPatternApi.Data.Entities;
using REPRPatternApi.Models.Requests;
using REPRPatternApi.Models.Responses;

namespace REPRPatternApi.Services;

public sealed class ProductService : IProductService
{
    private readonly ConcurrentDictionary<int, Product> _products;
    private int _nextId = 1;

    public ProductService()
    {
        _products = new ConcurrentDictionary<int, Product>();
        AddInitialProducts();
    }

    private void AddInitialProducts()
    {
        var initialProducts = new[]
        {
            new Product
            {
                Id = Interlocked.Increment(ref _nextId) - 1,
                Name = "Laptop",
                Description = "Gaming Laptop",
                Price = 1299.99m,
                Stock = 25,
            },
            new Product
            {
                Id = Interlocked.Increment(ref _nextId) - 1,
                Name = "Phone",
                Description = "Smartphone",
                Price = 899.99m,
                Stock = 50,
            },
            new Product
            {
                Id = Interlocked.Increment(ref _nextId) - 1,
                Name = "Headphones",
                Description = "Wireless Headphones",
                Price = 199.99m,
                Stock = 100,
            },
        };

        foreach (var product in initialProducts)
        {
            _products.TryAdd(product.Id, product);
        }
    }

    public ValueTask<IEnumerable<ProductResponse>> GetAllProductsAsync()
    {
        var productResponses = _products.Values.Select(p => new ProductResponse(
            p.Id,
            p.Name,
            p.Description,
            p.Price,
            p.Stock
        ));

        return ValueTask.FromResult(productResponses);
    }

    public ValueTask<ProductResponse?> GetProductByIdAsync(int id)
    {
        if (_products.TryGetValue(id, out var product))
        {
            return ValueTask.FromResult<ProductResponse?>(new ProductResponse(
                product.Id,
                product.Name,
                product.Description,
                product.Price,
                product.Stock
            ));
        }

        return ValueTask.FromResult<ProductResponse?>(null);
    }

    public ValueTask<ProductResponse> CreateProductAsync(CreateProductRequest request)
    {
        var product = new Product
        {
            Id = Interlocked.Increment(ref _nextId) - 1,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock,
        };

        if (!_products.TryAdd(product.Id, product))
        {
            throw new InvalidOperationException("Failed to add product");
        }

        return ValueTask.FromResult(new ProductResponse(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.Stock
        ));
    }

    public ValueTask<ProductResponse?> UpdateProductAsync(UpdateProductRequest request)
    {
        if (_products.TryGetValue(request.Id, out var existingProduct))
        {
            var updatedProduct = new Product
            {
                Id = existingProduct.Id,
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Stock = request.Stock,
            };

            if (_products.TryUpdate(request.Id, updatedProduct, existingProduct))
            {
                return ValueTask.FromResult<ProductResponse?>(new ProductResponse(
                    updatedProduct.Id,
                    updatedProduct.Name,
                    updatedProduct.Description,
                    updatedProduct.Price,
                    updatedProduct.Stock
                ));
            }
        }

        return ValueTask.FromResult<ProductResponse?>(null);
    }

    public ValueTask<bool> DeleteProductAsync(int id)
    {
        return ValueTask.FromResult(_products.TryRemove(id, out _));
    }
}
