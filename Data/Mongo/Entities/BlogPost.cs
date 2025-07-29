using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace REPRPatternApi.Data.Mongo.Entities
{
    public class Comment
    {
        public string Author { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime PostedAt { get; set; }
    }

    public class BlogPost
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public List<Comment> Comments { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
