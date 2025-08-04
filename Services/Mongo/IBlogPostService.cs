using REPRPatternApi.Models.Mongo.Requests;
using REPRPatternApi.Models.Mongo.Responses;

namespace REPRPatternApi.Services.Mongo;

public interface IBlogPostService
{
    ValueTask<BlogPostsResponse> GetAllBlogPostsAsync();
    ValueTask<BlogPostResponse?> GetBlogPostByIdAsync(string id);
    ValueTask<BlogPostResponse> CreateBlogPostAsync(BlogPostCreateRequest request);
    ValueTask<BlogPostResponse?> UpdateBlogPostAsync(string id, BlogPostUpdateRequest request);
    ValueTask<bool> DeleteBlogPostAsync(string id);
}
