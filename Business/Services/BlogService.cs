using Business.Services.Interfaces;
using Data.DTOs;
using Data.Entities;

namespace Business.Services;

public class BlogService : IBlogService
{
    private readonly ICosmosDbService _cosmosDbService;

    public BlogService(ICosmosDbService cosmosDbService)
    {
        _cosmosDbService = cosmosDbService;
    }

    public async Task AddPost(PostDto postDto)
    {
        var post = new Post
        {
            Id = Guid.NewGuid().ToString(),
            Author = postDto.Author,
            Title = postDto.Title,
            Content = postDto.Content
        };
        await _cosmosDbService.AddPostAsync(post, post.Id);
    }

    public async Task UpdatePost(Post post)
    {
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