using Business.Services.Interfaces;
using Data.Payloads;
using Data.Entities;

namespace Business.Services;

public class BlogService : IBlogService
{
    private readonly ICosmosDbService _cosmosDbService;

    public BlogService(ICosmosDbService cosmosDbService)
    {
        _cosmosDbService = cosmosDbService;
    }

    public async Task AddPost(PostPayload postPayload)
    {
        var post = new Post
        {
            Id = Guid.NewGuid().ToString(),
            Author = postPayload.Author,
            Title = postPayload.Title,
            Content = postPayload.Content
        };
        await _cosmosDbService.AddPostAsync(post, post.Id);
    }

    public async Task UpdatePost(Post post)
    {
        var existingPost = await GetPostById(post.Id);
        if (existingPost == null)
        {
            throw new Exception("Post not found");
        }

        if (post.ETag == null || post.ETag != existingPost.ETag)
        {
            throw new Exception("Post is outdated");
        }
        
        await _cosmosDbService.UpdatePostAsync(post, post.Id, post.Id);
    }

    public async Task<IEnumerable<Post>> GetAllPosts()
    {
        return await _cosmosDbService.GetAllPostsAsync();
    }

    public async Task<Post> GetPostById(string id)
    {
        return await _cosmosDbService.GetPostByIdAsync(id, id);
    }
}