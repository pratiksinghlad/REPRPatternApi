using REPRPatternApi.Models.Responses;
using REPRPatternApi.Services.Mongo;

namespace REPRPatternApi.Endpoints.Mongo;

/// <summary>
/// Endpoint to delete a blog post
/// </summary>
public sealed class DeleteBlogPostEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("blogposts/{id}", HandleAsync)
            .WithName("DeleteBlogPost")
            .WithSummary("Delete blog post")
            .WithDescription("Deletes a blog post and all its embedded comments")
            .WithTags("BlogPosts")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    /// <summary>
    /// Handles the request to delete a blog post
    /// </summary>
    /// <param name="id">The blog post ID</param>
    /// <param name="blogPostService">Blog post service for business logic</param>
    /// <param name="logger">Logger for structured logging</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content if successful or not found</returns>
    private static async Task<IResult> HandleAsync(
        string id,
        IBlogPostService blogPostService,
        ILogger<DeleteBlogPostEndpoint> logger,
        CancellationToken cancellationToken = default)
    {
        using var activity = logger.BeginScope(new Dictionary<string, object>
        {
            ["Operation"] = "DeleteBlogPost",
            ["BlogPostId"] = id,
            ["CorrelationId"] = Guid.NewGuid()
        });

        logger.LogInformation("Deleting blog post with ID {BlogPostId}", id);

        if (string.IsNullOrWhiteSpace(id))
        {
            logger.LogWarning("Invalid blog post ID provided: {BlogPostId}", id);
            return Results.BadRequest(new ErrorResponse("Blog post ID cannot be empty"));
        }

        try
        {
            var deleted = await blogPostService.DeleteBlogPostAsync(id);
            
            if (!deleted)
            {
                logger.LogWarning("Blog post with ID {BlogPostId} not found", id);
                return Results.NotFound(new ErrorResponse($"Blog post with ID {id} not found"));
            }

            logger.LogInformation("Successfully deleted blog post with ID {BlogPostId}", id);
            return Results.NoContent();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while deleting blog post with ID {BlogPostId}", id);
            return Results.Problem(
                title: "Internal Server Error",
                detail: $"An error occurred while deleting blog post with ID {id}",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
