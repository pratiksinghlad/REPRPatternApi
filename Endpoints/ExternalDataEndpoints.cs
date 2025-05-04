using REPRPatternApi.Services;

namespace REPRPatternApi.Endpoints;

public class ExternalDataEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("externaldata");

        group.MapGet("/", GetExternalDataAsync)
            .WithName("GetExternalData")
            .Produces<YourResponseModel>(StatusCodes.Status200OK);
    }

    private async Task<IResult> GetExternalDataAsync(IExternalApiService externalApiService)
    {
        // Call the external API with resilience policies
        var data = await externalApiService.GetAsync<YourResponseModel>("https://api.github.com/users/octocat");
        return Results.Ok(data);
    }
}

// Example model for your API response
public class YourResponseModel
{
    public int Id { get; set; }
    public string Name { get; set; }

    public string Login { get; set; }
    // Other properties
}