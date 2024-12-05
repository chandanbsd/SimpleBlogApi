using Data.Payloads;
using Data.Entities;

namespace Business.Services.Interfaces;

public interface IBlogService
{
    Task AddPost(PostPayload post);

    Task UpdatePost(Post post);

    Task<IEnumerable<Post>> GetAllPosts();

    Task<Post> GetPostById(string id);
}