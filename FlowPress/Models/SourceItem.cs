

namespace FlowPress.Models;

public class SourceItem
{
    public int Id { get; set; }
    public int SourceId { get; set; }
    public string Json { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navegacion......
    public IngestModels? Source { get; set; }
}