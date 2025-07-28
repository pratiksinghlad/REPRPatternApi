using Microsoft.AspNetCore.Mvc;
using REPRPatternApi.Extensions;
using REPRPatternApi.Models.Mongo.Requests;
using REPRPatternApi.Models.Mongo.Responses;
using REPRPatternApi.Services.Mongo;

namespace REPRPatternApi.Endpoints.Mongo;

/// <summary>
/// Endpoint to create a new blog post
/// </summary>
public sealed class CreateBlogPostEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("blogposts", HandleAsync)
            .WithName("CreateBlogPost")
            .WithSummary("Create a new blog post")
            .WithDescription("Creates a new blog post with embedded comments using MongoDB")
            .WithTags("BlogPosts")
            .Produces<BlogPostResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    /// <summary>
    /// Handles the request to create a new blog post
    /// </summary>
    /// <param name="request">Create blog post request</param>
    /// <param name="blogPostService">Blog post service for business logic</param>
    /// <param name="linkGenerator">Link generator for location header</param>
    /// <param name="httpContext">HTTP context</param>
    /// <param name="logger">Logger for structured logging</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created blog post response</returns>
    private static async Task<IResult> HandleAsync(
        [FromBody] BlogPostCreateRequest request,
        IBlogPostService blogPostService,
        LinkGenerator linkGenerator,
        HttpContext httpContext,
        ILogger<CreateBlogPostEndpoint> logger,
        CancellationToken cancellationToken = default)
    {
        using var activity = logger.BeginScope(new Dictionary<string, object>
        {
            ["Operation"] = "CreateBlogPost",
            ["BlogPostTitle"] = request.Title,
            ["CorrelationId"] = Guid.NewGuid()
        });

        logger.LogInformation("Creating new blog post: {BlogPostTitle}", request.Title);

        // Validate the request
        var validationResult = ValidationExtensions.ValidateRequest(request);
        if (validationResult != null)
        {
            logger.LogWarning("Validation failed for create blog post request");
            return validationResult;
        }

        try
        {
            var response = await blogPostService.CreateBlogPostAsync(request);

            var locationUri = linkGenerator.GetUriByName(
                httpContext,
                "GetBlogPostById",
                new { id = response.Id }) ?? $"/api/v1/blogposts/{response.Id}";

            logger.LogInformation("Successfully created blog post with ID {BlogPostId}", response.Id);

            return Results.Created(locationUri, response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while creating blog post: {BlogPostTitle}", request.Title);
            return Results.Problem(
                title: "Internal Server Error",
                detail: "An error occurred while creating the blog post",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
