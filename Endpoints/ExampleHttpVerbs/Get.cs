using REPRPatternApi.Endpoints;

namespace REPRPatternApi.Endpoints.ExampleHttpVerbs;

public class Get : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("get", () => "Get endpoint");
    }
}