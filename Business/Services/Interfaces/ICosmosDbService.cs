using Data.Entities;

namespace Business.Services.Interfaces;

public interface ICosmosDbService
{
    Task AddPostAsync<T>(T item, string partitionKey);

    Task UpdatePostAsync<T>(T item, string id, string partitionKey);

    Task<IEnumerable<Post>> GetAllPostsAsync();

    Task<Post> GetPostByIdAsync(string id, string partitionKey);

    Task<IEnumerable<PostVersion>> GetAuditTrail(string postId);
}