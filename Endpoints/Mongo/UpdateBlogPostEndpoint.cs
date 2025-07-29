using Microsoft.AspNetCore.Mvc;
using REPRPatternApi.Extensions;
using REPRPatternApi.Models.Mongo.Requests;
using REPRPatternApi.Models.Mongo.Responses;
using REPRPatternApi.Models.Responses;
using REPRPatternApi.Services.Mongo;

namespace REPRPatternApi.Endpoints.Mongo;

/// <summary>
/// Endpoint to update a blog post
/// </summary>
public sealed class UpdateBlogPostEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("blogposts/{id}", HandleAsync)
            .WithName("UpdateBlogPost")
            .WithSummary("Update blog post")
            .WithDescription("Updates an existing blog post with embedded comments")
            .WithTags("BlogPosts")
            .Produces<BlogPostResponse>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    /// <summary>
    /// Handles the request to update a blog post
    /// </summary>
    /// <param name="id">The blog post ID</param>
    /// <param name="request">Update blog post request</param>
    /// <param name="blogPostService">Blog post service for business logic</param>
    /// <param name="logger">Logger for structured logging</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated blog post response or not found</returns>
    private static async Task<IResult> HandleAsync(
        string id,
        [FromBody] BlogPostUpdateRequest request,
        IBlogPostService blogPostService,
        ILogger<UpdateBlogPostEndpoint> logger,
        CancellationToken cancellationToken = default)
    {
        using var activity = logger.BeginScope(new Dictionary<string, object>
        {
            ["Operation"] = "UpdateBlogPost",
            ["BlogPostId"] = id,
            ["BlogPostTitle"] = request.Title,
            ["CorrelationId"] = Guid.NewGuid()
        });

        logger.LogInformation("Updating blog post with ID {BlogPostId}", id);

        if (string.IsNullOrWhiteSpace(id))
        {
            logger.LogWarning("Invalid blog post ID provided: {BlogPostId}", id);
            return Results.BadRequest(new ErrorResponse("Blog post ID cannot be empty"));
        }

        // Validate the request
        var validationResult = ValidationExtensions.ValidateRequest(request);
        if (validationResult != null)
        {
            logger.LogWarning("Validation failed for update blog post request");
            return validationResult;
        }

        try
        {
            var response = await blogPostService.UpdateBlogPostAsync(id, request);
            
            if (response == null)
            {
                logger.LogWarning("Blog post with ID {BlogPostId} not found", id);
                return Results.NotFound(new ErrorResponse($"Blog post with ID {id} not found"));
            }

            logger.LogInformation("Successfully updated blog post with ID {BlogPostId}", id);
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while updating blog post with ID {BlogPostId}", id);
            return Results.Problem(
                title: "Internal Server Error",
                detail: $"An error occurred while updating blog post with ID {id}",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
