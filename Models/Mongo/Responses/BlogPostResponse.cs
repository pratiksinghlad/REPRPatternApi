using REPRPatternApi.Data.Mongo.Entities;

namespace REPRPatternApi.Models.Mongo.Responses
{
    public class BlogPostResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public List<CommentResponse> Comments { get; set; } = new();
    }

    public class CommentResponse
    {
        public string Author { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime PostedAt { get; set; }
    }

    public class BlogPostsResponse
    {
        public List<BlogPostResponse> BlogPosts { get; set; } = new();
        public int TotalCount { get; set; }
    }
}
