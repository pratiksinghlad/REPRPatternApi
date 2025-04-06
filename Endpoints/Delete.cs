using REPRPatternApi.Endpoints;

namespace MinimalEndpoints.Endpoints;

public class Delete : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("delete", () => "Delete endpoint");
    }
}