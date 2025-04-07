namespace REPRPatternApi.Endpoints.ExampleHttpVerbs;

public class Post : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("post", () => "Post endpoint");
    }
}