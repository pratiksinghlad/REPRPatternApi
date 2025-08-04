using System.ComponentModel.DataAnnotations;

namespace REPRPatternApi.Models.Mongo.Requests
{
    public class BlogPostUpdateRequest
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public required string Title { get; set; }

        [Required(ErrorMessage = "Body is required")]
        [StringLength(10000, ErrorMessage = "Body cannot exceed 10000 characters")]
        public required string Body { get; set; }

        public List<CommentRequest>? Comments { get; set; }
    }
}
