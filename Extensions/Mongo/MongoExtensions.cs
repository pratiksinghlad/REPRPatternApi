using REPRPatternApi.Data.Mongo.Repositories;
using REPRPatternApi.Services.Mongo;
using MongoDB.Driver;

namespace REPRPatternApi.Extensions.Mongo
{
    public static class MongoExtensions
    {
        public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MongoDb");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("MongoDB connection string is not configured.");
            }

            // Register MongoClient as singleton
            services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));
            
            // Register database using the client
            services.AddSingleton<IMongoDatabase>(provider =>
            {
                var client = provider.GetRequiredService<IMongoClient>();
                var mongoUrl = new MongoUrl(connectionString);
                return client.GetDatabase(mongoUrl.DatabaseName ?? "REPRPatternDb");
            });

            services.AddScoped<BlogPostRepository>();
            services.AddScoped<IBlogPostService, BlogPostService>();
            
            return services;
        }
    }
}
