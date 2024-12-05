using Data.Entities;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Business.Services;

public class ChangeFeedService
{
    private readonly ILogger<ChangeFeedService> _logger;
    private Container _auditContainer;
    private Container _leaseContainer;
    private Container _postsContainer;


    public ChangeFeedService(
        IConfiguration configuration,
        ILogger<ChangeFeedService> logger)
    {
        _logger = logger;
        InitializeChangeFeedAsync(configuration).GetAwaiter().GetResult();
    }

    private async Task InitializeChangeFeedAsync(IConfiguration configuration)
    {
        var cosmosClient = new CosmosClient(configuration["CosmosDb:Account"], configuration["CosmosDb:Key"]);
        var database = cosmosClient.GetDatabase(configuration["CosmosDb:DatabaseName"]);
        _postsContainer = database.GetContainer(configuration["CosmosDb:PostsContainer"]);
        _leaseContainer =
            await database.CreateContainerIfNotExistsAsync(configuration["CosmosDb:LeaseContainer"], "/id");
        _auditContainer =
            await database.CreateContainerIfNotExistsAsync(configuration["CosmosDb:AuditContainer"], "/PostId");
    }

    public async Task StartChangeFeedProcessorAsync()
    {
        var processor = _postsContainer
            .GetChangeFeedProcessorBuilder<Post>("changeFeedProcessor", HandleChangesAsync)
            .WithInstanceName("changeFeedInstance")
            .WithLeaseContainer(_leaseContainer)
            .Build();

        await processor.StartAsync();
    }

    private async Task HandleChangesAsync(IReadOnlyCollection<Post> changes, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Detected {changes.Count} changes.");
        foreach (var post in changes)
        {
            var postVersion = new PostVersion
            {
                Id = Guid.NewGuid().ToString(),
                PostId = post.Id,
                Author = post.Author,
                Title = post.Title,
                Content = post.Content
            };
            _logger.LogInformation($"Creating item in changes container for post ID: {post.Id}");

            try
            {
                var item = await _auditContainer.CreateItemAsync(postVersion, new PartitionKey(postVersion.PostId));
                _logger.LogInformation($"Created item in changes container with ID: {item.Resource.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    $"Error creating item in changes container for post ID: {post.Id}. Error: {ex.Message}");
            }
        }
    }
}