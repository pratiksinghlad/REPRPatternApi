namespace REPRPatternApi.Endpoints.ExampleHttpVerbs;

public class Put : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("put", () => "Put endpoint");
    }
}