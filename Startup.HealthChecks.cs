using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;

namespace REPRPatternApi;

public partial class Startup
{
    /// <summary>
    /// Load healthchecks 
    /// </summary>
    /// <param name="services"></param>
    public void LoadHealthChecks(IServiceCollection services)
    {
        
    }

    private static Task WriteResponse(HttpContext httpContext, HealthReport result)
    {
        httpContext.Response.ContentType = "application/json";

        var obj = new
        {
            status = result.Status.ToString(),
            results = result.Entries.Select(pair => new
            {
                source = pair.Key,
                status = pair.Value.Status.ToString(),
                description = pair.Value.Description
            })
        };

        return httpContext.Response.WriteAsync(JsonConvert.SerializeObject(obj, Formatting.Indented));
    }
}
