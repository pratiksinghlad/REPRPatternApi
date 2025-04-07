namespace REPRPatternApi.Endpoints.ExampleHttpVerbs;

public class Delete : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("delete", () => "Delete endpoint");
    }
}