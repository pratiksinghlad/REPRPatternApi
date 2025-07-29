using REPRPatternApi.Data.Mongo.Entities;
using REPRPatternApi.Data.Mongo.Repositories;
using REPRPatternApi.Models.Mongo.Requests;
using REPRPatternApi.Models.Mongo.Responses;

namespace REPRPatternApi.Services.Mongo;

public class BlogPostService : IBlogPostService
{
    private readonly BlogPostRepository _repository;
    private readonly ILogger<BlogPostService> _logger;

    public BlogPostService(BlogPostRepository repository, ILogger<BlogPostService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async ValueTask<BlogPostsResponse> GetAllBlogPostsAsync()
    {
        _logger.LogInformation("Retrieving all blog posts from service layer");
        
        var blogPosts = await _repository.GetAllAsync();
        
        return new BlogPostsResponse
        {
            BlogPosts = blogPosts.ConvertAll(MapToResponse),
            TotalCount = blogPosts.Count
        };
    }

    public async ValueTask<BlogPostResponse?> GetBlogPostByIdAsync(string id)
    {
        _logger.LogInformation("Retrieving blog post with ID {BlogPostId} from service layer", id);
        
        var blogPost = await _repository.GetByIdAsync(id);
        
        return blogPost == null ? null : MapToResponse(blogPost);
    }

    public async ValueTask<BlogPostResponse> CreateBlogPostAsync(BlogPostCreateRequest request)
    {
        _logger.LogInformation("Creating blog post {Title} in service layer", request.Title);
        
        var blogPost = new BlogPost
        {
            Title = request.Title,
            Body = request.Body,
            Comments = request.Comments?.ConvertAll(c => new Comment
            {
                Author = c.Author,
                Content = c.Content,
                PostedAt = DateTime.UtcNow
            }) ?? new List<Comment>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdPost = await _repository.CreateAsync(blogPost);
        return MapToResponse(createdPost);
    }

    public async ValueTask<BlogPostResponse?> UpdateBlogPostAsync(string id, BlogPostUpdateRequest request)
    {
        _logger.LogInformation("Updating blog post with ID {BlogPostId} in service layer", id);
        
        // Check if blog post exists
        var existingPost = await _repository.GetByIdAsync(id);
        if (existingPost == null)
        {
            return null;
        }

        var updatedPost = new BlogPost
        {
            Id = id,
            Title = request.Title,
            Body = request.Body,
            Comments = request.Comments?.ConvertAll(c => new Comment
            {
                Author = c.Author,
                Content = c.Content,
                PostedAt = DateTime.UtcNow
            }) ?? new List<Comment>(),
            CreatedAt = existingPost.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };

        var result = await _repository.UpdateAsync(id, updatedPost);
        return result == null ? null : MapToResponse(result);
    }

    public async ValueTask<bool> DeleteBlogPostAsync(string id)
    {
        _logger.LogInformation("Deleting blog post with ID {BlogPostId} in service layer", id);
        
        return await _repository.DeleteAsync(id);
    }

    private static BlogPostResponse MapToResponse(BlogPost blogPost)
    {
        return new BlogPostResponse
        {
            Id = blogPost.Id,
            Title = blogPost.Title,
            Body = blogPost.Body,
            Comments = blogPost.Comments.ConvertAll(c => new CommentResponse
            {
                Author = c.Author,
                Content = c.Content,
                PostedAt = c.PostedAt
            })
        };
    }
}
