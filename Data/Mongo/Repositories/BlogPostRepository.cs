using REPRPatternApi.Data.Mongo.Entities;
using MongoDB.Driver;

namespace REPRPatternApi.Data.Mongo.Repositories
{
    public class BlogPostRepository
    {
        private readonly IMongoCollection<BlogPost> _collection;

        public BlogPostRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<BlogPost>("BlogPosts");
        }

        public async Task<BlogPost> CreateAsync(BlogPost post)
        {
            await _collection.InsertOneAsync(post);
            return post;
        }

        public async Task<BlogPost?> GetByIdAsync(string id)
        {
            return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<BlogPost>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<BlogPost?> UpdateAsync(string id, BlogPost post)
        {
            post.UpdatedAt = DateTime.UtcNow;
            var result = await _collection.ReplaceOneAsync(x => x.Id == id, post);
            return result.MatchedCount > 0 ? post : null;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _collection.DeleteOneAsync(x => x.Id == id);
            return result.DeletedCount > 0;
        }
    }
}
