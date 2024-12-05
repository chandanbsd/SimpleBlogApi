using Newtonsoft.Json;

namespace Data.Entities;

public class PostVersion
{
    [JsonProperty("id")] public required string Id { get; set; }

    [JsonProperty("PostId")] public required string PostId { get; set; }

    public required string Author { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}