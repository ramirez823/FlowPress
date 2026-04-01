using System.Text.Json;
using System.Text.Json.Serialization;

namespace FlowPress.Models.ImportExport;

public class IngestRoot
{
    [JsonPropertyName("schemaVersion")]
    public string SchemaVersion { get; set; } = string.Empty;

    [JsonPropertyName("exportedAt")]
    public DateTime ExportedAt { get; set; }

    [JsonPropertyName("source")]
    public SourceSchema Source { get; set; } = new();

    [JsonPropertyName("normalized")]
    public NormalizedContentSchema Normalized { get; set; } = new();

    [JsonPropertyName("raw")]
    public RawContentSchema? Raw { get; set; }
}

public class SourceSchema
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("requiresSecret")]
    public bool RequiresSecret { get; set; }
}

public class NormalizedContentSchema
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("externalId")]
    public string? ExternalId { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    [JsonPropertyName("summary")]
    public string? Summary { get; set; }

    [JsonPropertyName("publishedAt")]
    public DateTime? PublishedAt { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("author")]
    public string? Author { get; set; }

    [JsonPropertyName("language")]
    public string? Language { get; set; }

    [JsonPropertyName("category")]
    public CategorySchema? Category { get; set; }
}

public class CategorySchema
{
    [JsonPropertyName("primary")]
    public string? Primary { get; set; }

    [JsonPropertyName("secondary")]
    public List<string> Secondary { get; set; } = new();
}

public class RawContentSchema
{
    [JsonPropertyName("format")]
    public string? Format { get; set; }

    [JsonPropertyName("data")]
    public JsonElement Data { get; set; }
}