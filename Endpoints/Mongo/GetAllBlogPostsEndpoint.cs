using REPRPatternApi.Models.Mongo.Responses;
using REPRPatternApi.Services.Mongo;

namespace REPRPatternApi.Endpoints.Mongo;

/// <summary>
/// Endpoint to retrieve all blog posts
/// </summary>
public sealed class GetAllBlogPostsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("blogposts", HandleAsync)
            .WithName("GetAllBlogPosts")
            .WithSummary("Get all blog posts")
            .WithDescription("Retrieves all blog posts with embedded comments")
            .WithTags("BlogPosts")
            .Produces<BlogPostsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    /// <summary>
    /// Handles the request to get all blog posts
    /// </summary>
    /// <param name="blogPostService">Blog post service for business logic</param>
    /// <param name="logger">Logger for structured logging</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>All blog posts response</returns>
    private static async Task<IResult> HandleAsync(
        IBlogPostService blogPostService,
        ILogger<GetAllBlogPostsEndpoint> logger,
        CancellationToken cancellationToken = default)
    {
        using var activity = logger.BeginScope(new Dictionary<string, object>
        {
            ["Operation"] = "GetAllBlogPosts",
            ["CorrelationId"] = Guid.NewGuid()
        });

        logger.LogInformation("Retrieving all blog posts");

        try
        {
            var response = await blogPostService.GetAllBlogPostsAsync();

            logger.LogInformation("Successfully retrieved {Count} blog posts", response.TotalCount);
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while retrieving all blog posts");
            return Results.Problem(
                title: "Internal Server Error",
                detail: "An error occurred while retrieving blog posts",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
