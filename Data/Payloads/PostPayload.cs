namespace Data.Payloads;

public class PostPayload
{
    public required string Author { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    
    public string? ETag { get; set; }
}