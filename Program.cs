using Asp.Versioning.Builder;
using Asp.Versioning;
using REPRPatternApi.Extensions;
using Microsoft.AspNetCore.ResponseCompression;
using Scalar.AspNetCore;
using REPRPatternApi.Models.Requests;
using REPRPatternApi.Models.Responses;
using System.Text.Json.Serialization;
using REPRPatternApi.Services;
using REPRPatternApi;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true; // This is important for https, by default it is disabled for https.
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
    new[] {
        "application/json",
        "application/javascript",
        "text/css",
        "text/html",
        "text/json",
        "text/plain",
        "text/xml"
    });
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Fastest;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Fastest;
});

builder.Services.AddEndpoints(typeof(Program).Assembly);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
        .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1);
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddEndpoints(typeof(Program).Assembly);

// Configure JSON options to use the generated context
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddResponseCaching();

ScalarDoc.LoadScalar(builder.Services);

// Health checks should be added later
//LoadHealthChecks(builder.Services);

builder.Services.AddMemoryCache();
builder.Services.AddApiVersioning(options => options.ReportApiVersions = true);

builder.Services.AddEndpoints(typeof(Program).Assembly);

WebApplication app = builder.Build();

ApiVersionSet apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    .ReportApiVersions()
    .Build();

RouteGroupBuilder versionedGroup = app
    .MapGroup("api/v{version:apiVersion}")
    .WithApiVersionSet(apiVersionSet);

app.MapEndpoints(versionedGroup);

app.UseResponseCompression();
app.UseStatusCodePages();
app.UseRouting();
app.UseCors("AllowAll");

ScalarDoc.UseScalar(ref app);

app.UseEndpoints(endpoints =>
{
    endpoints.MapScalarApiReference(opt =>
    {
        opt.Title = $"REPR Pattern Api Documentation - {app.Environment.EnvironmentName}";
        if (app.Environment.IsDevelopment())
            opt.Theme = ScalarTheme.DeepSpace;
        else if (app.Environment.IsStaging())
            opt.Theme = ScalarTheme.BluePlanet;
        else
            opt.Theme = ScalarTheme.Purple;
    });
});

app.UseCookiePolicy();
app.UseHttpsRedirection();
app.Run();



[JsonSerializable(typeof(UpdateProductRequest))]
[JsonSerializable(typeof(CreateProductRequest))]
[JsonSerializable(typeof(ProductResponse))]
[JsonSerializable(typeof(ProductsResponse))]
[JsonSerializable(typeof(ErrorResponse))]
[JsonSerializable(typeof(Dictionary<string, string[]>))]
public partial class AppJsonSerializerContext : JsonSerializerContext
{
}