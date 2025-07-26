using System.IO.Compression;
using System.Text.Json.Serialization;
using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using REPRPatternApi.Extensions;
using REPRPatternApi.Models;
using REPRPatternApi.Models.Requests;
using REPRPatternApi.Models.Responses;
using REPRPatternApi.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Configuration
    .AddJsonFile("appsettings.json", false, true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
    .AddJsonFile("secrets/appsettings.secrets.json", true, true)
    .AddEnvironmentVariables();

// API versioning
builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1);
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
        options.ReportApiVersions = true;
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'V";
        options.SubstituteApiVersionInUrl = true;
    });

// Memory cache and distributed cache for response caching
builder.Services.AddMemoryCache();
builder.Services.AddResponseCaching();
builder.Services.AddDistributedMemoryCache();

// Compression (Brotli and Gzip)
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
        {
            "application/json",
            "application/javascript",
            "text/css",
            "text/html",
            "text/json",
            "text/plain",
            "text/xml",
        });
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

// Health checks
builder.Services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy());

// Core services
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.Configure<ExternalApiSettings>(
    builder.Configuration.GetSection(nameof(ExternalApiSettings)));

// JSON configuration
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", 
        policy => policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Register application services
builder.Services.AddExternalApiHttpClient(
    builder.Configuration.GetValue<string>("ExternalApiSettings:BaseUrl")!);

builder.Services.AddScoped<IProductService, ProductService>();

// Add FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Add endpoints from assembly
builder.Services.AddEndpoints(typeof(Program).Assembly);

// Documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.Title = "API Documentation";
    config.Version = "v1";
    config.Description = "API Documentation using Scalar";
    config.DocumentName = "v1";
});

var app = builder.Build();

// Security headers middleware
app.Use(async (context, next) =>
    {
        context.Response.Headers.Remove("Server");
        context.Response.Headers.Remove("X-Powered-By");
        context.Response.Headers.Remove("X-AspNet-Version");
        context.Response.Headers.Remove("X-SourceFiles");
        context.Response.Headers.Remove("X-Runtime");
    
        // Add security headers
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";
        context.Response.Headers["X-Frame-Options"] = "DENY";
        context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
        context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    
        await next.Invoke();
    });

// Middleware pipeline
app.UseResponseCompression();
app.UseResponseCaching();
app.UseStatusCodePages();

// Add before app.UseRouting()
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAll");

// Configure API versioning
var apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    .ReportApiVersions()
    .Build();

var versionedGroup = app.MapGroup("api/v{version:apiVersion}")
    .WithApiVersionSet(apiVersionSet);

// Map endpoints
app.MapEndpoints(versionedGroup);

// Map health checks
app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = _ => true });
app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => false });

// Configure OpenAPI and documentation
app.UseOpenApi(options => options.Path = "/openapi/v1.json");

app.UseEndpoints(endpoints =>
{
    endpoints.MapScalarApiReference(opt =>
    {
        opt.Title = $"REPR Pattern Api Documentation - {app.Environment.EnvironmentName}";
        opt.Theme = app.Environment.IsDevelopment() 
            ? ScalarTheme.DeepSpace 
            : app.Environment.IsStaging() 
                ? ScalarTheme.BluePlanet 
                : ScalarTheme.Purple;
    });
});

await app.RunAsync();

// Source generation for JSON serialization
[JsonSerializable(typeof(UpdateProductRequest))]
[JsonSerializable(typeof(CreateProductRequest))]
[JsonSerializable(typeof(ProductResponse))]
[JsonSerializable(typeof(ProductsResponse))]
[JsonSerializable(typeof(ErrorResponse))]
[JsonSerializable(typeof(Dictionary<string, string[]>))]
public partial class AppJsonSerializerContext : JsonSerializerContext { }