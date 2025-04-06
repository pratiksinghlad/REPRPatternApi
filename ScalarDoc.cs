﻿namespace REPRPatternApi;

public class ScalarDoc
{
    /// <summary>
    /// Load Scalar settings
    /// </summary>
    /// <param name="services"></param>
    public static void LoadScalar(IServiceCollection services)
    {
        // Configure OpenAPI - use a single approach with Scalar
        services.AddEndpointsApiExplorer();
        services.AddOpenApiDocument(config =>
        {
            config.Title = "API Documentation";
            config.Version = "v1";
            config.Description = "API Documentation using Scalar";
            config.DocumentName = "v1";
        });
    }

    /// <summary>
    /// Add Scalar to the hosted service
    /// </summary>
    /// <param name="app"></param>
    public static void UseScalar(ref WebApplication app)
    {
        app.UseOpenApi(options =>
        {
            options.Path = "/openapi/v1.json";
        });
    }
}
