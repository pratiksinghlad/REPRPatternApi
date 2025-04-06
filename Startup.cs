using Asp.Versioning;
using Autofac;
using Microsoft.AspNetCore.ResponseCompression;
using REPRPatternApi.Endpoints;
using REPRPatternApi.Extensions;
using Scalar.AspNetCore;

namespace REPRPatternApi;

/// <summary>
/// Startup class called from Program.cs
/// </summary>
public partial class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public static void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterModule<Registrations>();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddResponseCompression(options =>
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

        services.Configure<BrotliCompressionProviderOptions>(options =>
        {
            options.Level = System.IO.Compression.CompressionLevel.Fastest;
        });

        services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = System.IO.Compression.CompressionLevel.Fastest;
        });

        services.AddEndpoints(typeof(Program).Assembly);

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1);
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'V";
            options.SubstituteApiVersionInUrl = true;
        });

        services.AddEndpoints(typeof(Program).Assembly);

        // Configure JSON options to use the generated context
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
        });

        services.AddHttpContextAccessor();
        services.AddResponseCaching();

        LoadScalar(services);

        // Health checks should be added later
        LoadHealthChecks(services);

        services.AddMemoryCache();
        services.AddApiVersioning(options => options.ReportApiVersions = true);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseResponseCompression();
        app.UseStatusCodePages();
        app.UseRouting();
        app.UseCors("AllowAll");

        UseScalar(ref app);


        app.UseEndpoints(endpoints =>
        {
            var apiVersionSet = endpoints.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();

            var versionedGroup = endpoints.MapGroup("api/v1")
                .WithApiVersionSet(apiVersionSet);

            new ProductEndpoints().MapEndpoint(versionedGroup);

            endpoints.MapScalarApiReference(opt =>
            {
                opt.Title = $"REPR Pattern Api Documentation - {env.EnvironmentName}";
                if (env.IsDevelopment())
                    opt.Theme = ScalarTheme.DeepSpace;
                else if (env.IsStaging())
                    opt.Theme = ScalarTheme.BluePlanet;
                else
                    opt.Theme = ScalarTheme.Purple;
            });
        });

        app.UseCookiePolicy();
    }
}