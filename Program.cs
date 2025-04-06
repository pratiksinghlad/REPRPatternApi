using System.Text.Json.Serialization;
using REPRPatternApi.Models.Requests;
using REPRPatternApi.Models.Responses;
using REPRPatternApi;
using Autofac.Extensions.DependencyInjection;

[JsonSerializable(typeof(UpdateProductRequest))]
[JsonSerializable(typeof(CreateProductRequest))]
[JsonSerializable(typeof(ProductResponse))]
[JsonSerializable(typeof(ProductsResponse))]
[JsonSerializable(typeof(ErrorResponse))]
[JsonSerializable(typeof(Dictionary<string, string[]>))]
public partial class AppJsonSerializerContext : JsonSerializerContext
{
    
}

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    /// <summary>
    /// Create host
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureWebHostDefaults(webBuilder =>
            {

                webBuilder.UseStartup<Startup>();
            })
            .ConfigureAppConfiguration((context, builder) =>
            {
                builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                builder.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", true, true);
                builder.AddJsonFile("secrets/appsettings.secrets.json", true, true);
                builder.AddUserSecrets<Program>();
                builder.AddEnvironmentVariables();
            });
}