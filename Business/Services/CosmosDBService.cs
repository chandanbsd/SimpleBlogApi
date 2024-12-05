using Business.Services.Interfaces;
using Data.Entities;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Container = Microsoft.Azure.Cosmos.Container;

namespace Business.Services;

public class CosmosDbService : ICosmosDbService
{
    private readonly ChangeFeedService _changeFeedService;
    private Container _postsContainer;

    public CosmosDbService(IConfiguration configuration, ChangeFeedService changeFeedService)
    {
        _changeFeedService = changeFeedService;
        InitializeCosmosDbAsync(configuration).GetAwaiter().GetResult();
    }

    public async Task AddPostAsync<T>(T item, string partitionKey)
    {
        await _postsContainer.CreateItemAsync(item, new PartitionKey(partitionKey));
    }

    public async Task UpdatePostAsync<T>(T item, string id, string partitionKey)
    {
        await _postsContainer.ReplaceItemAsync(item, id, new PartitionKey(partitionKey));
    }

    public async Task<IEnumerable<Post>> GetAllPostsAsync()
    {
        var query = "SELECT * FROM c";
        return await QueryPostsAsync<Post>(query);
    }

    public async Task<Post> GetPostByIdAsync(string id, string partitionKey)
    {
        var response = await _postsContainer.ReadItemAsync<Post>(id, new PartitionKey(partitionKey));
        return response.Resource;
    }

    private async Task InitializeCosmosDbAsync(IConfiguration configuration)
    {
        var cosmosClient = new CosmosClient(configuration["CosmosDb:Account"], configuration["CosmosDb:Key"]);
        var databaseResponse =
            await cosmosClient.CreateDatabaseIfNotExistsAsync(configuration["CosmosDb:DatabaseName"]);
        _postsContainer =
            await databaseResponse.Database.CreateContainerIfNotExistsAsync(configuration["CosmosDb:PostsContainer"],
                "/id");
        await _changeFeedService.StartChangeFeedProcessorAsync();
    }

    private async Task<IEnumerable<T>> QueryPostsAsync<T>(string query)
    {
        var iterator = _postsContainer.GetItemQueryIterator<T>(new QueryDefinition(query));
        var results = new List<T>();
        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            results.AddRange(response.ToList());
        }

        return results;
    }
}