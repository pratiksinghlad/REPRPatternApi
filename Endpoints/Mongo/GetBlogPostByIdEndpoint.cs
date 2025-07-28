using REPRPatternApi.Models.Mongo.Responses;
using REPRPatternApi.Models.Responses;
using REPRPatternApi.Services.Mongo;

namespace REPRPatternApi.Endpoints.Mongo;

/// <summary>
/// Endpoint to retrieve a blog post by its ID
/// </summary>
public sealed class GetBlogPostByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("blogposts/{id}", HandleAsync)
            .WithName("GetBlogPostById")
            .WithSummary("Get blog post by ID")
            .WithDescription("Retrieves a specific blog post with embedded comments by its unique identifier")
            .WithTags("BlogPosts")
            .Produces<BlogPostResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    /// <summary>
    /// Handles the request to get a blog post by ID
    /// </summary>
    /// <param name="id">The blog post ID</param>
    /// <param name="blogPostService">Blog post service for business logic</param>
    /// <param name="logger">Logger for structured logging</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Blog post response or not found</returns>
    private static async Task<IResult> HandleAsync(
        string id,
        IBlogPostService blogPostService,
        ILogger<GetBlogPostByIdEndpoint> logger,
        CancellationToken cancellationToken = default)
    {
        using var activity = logger.BeginScope(new Dictionary<string, object>
        {
            ["Operation"] = "GetBlogPostById",
            ["BlogPostId"] = id,
            ["CorrelationId"] = Guid.NewGuid()
        });

        logger.LogInformation("Retrieving blog post with ID {BlogPostId}", id);

        if (string.IsNullOrWhiteSpace(id))
        {
            logger.LogWarning("Invalid blog post ID provided: {BlogPostId}", id);
            return Results.BadRequest(new ErrorResponse("Blog post ID cannot be empty"));
        }

        try
        {
            var response = await blogPostService.GetBlogPostByIdAsync(id);
            
            if (response == null)
            {
                logger.LogWarning("Blog post with ID {BlogPostId} not found", id);
                return Results.NotFound(new ErrorResponse($"Blog post with ID {id} not found"));
            }

            logger.LogInformation("Successfully retrieved blog post with ID {BlogPostId}", id);
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while retrieving blog post with ID {BlogPostId}", id);
            return Results.Problem(
                title: "Internal Server Error",
                detail: $"An error occurred while retrieving blog post with ID {id}",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
