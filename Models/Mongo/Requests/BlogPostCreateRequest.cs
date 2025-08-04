using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace REPRPatternApi.Models.Mongo.Requests
{
    public class BlogPostCreateRequest
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public required string Title { get; set; }

        [Required(ErrorMessage = "Body is required")]
        [StringLength(10000, ErrorMessage = "Body cannot exceed 10000 characters")]
        public required string Body { get; set; }

        public List<CommentRequest>? Comments { get; set; }
    }

    public class CommentRequest
    {
        [Required(ErrorMessage = "Author is required")]
        [StringLength(100, ErrorMessage = "Author cannot exceed 100 characters")]
        public required string Author { get; set; }

        [Required(ErrorMessage = "Content is required")]
        [StringLength(1000, ErrorMessage = "Content cannot exceed 1000 characters")]
        public required string Content { get; set; }
    }
}
