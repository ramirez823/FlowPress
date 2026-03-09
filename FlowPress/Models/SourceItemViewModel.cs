using System.Text.Json;
using System.Text.Json.Serialization;

namespace FlowPress.Models;

public class SourceItemViewModel
{
    public Source? Source { get; set; }
    public NormalizedContent? Normalized { get; set; }
    public SourceSchema? Schema { get; set; }
    public RawContent? Raw { get; set; }

    public SourceItemViewModel(SourceItem item)
    {
        Source = item.Source;
        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var doc = JsonDocument.Parse(item.Json);
            var root = doc.RootElement;

            if (root.TryGetProperty("normalized", out var norm))
                Normalized = JsonSerializer.Deserialize<NormalizedContent>(norm.GetRawText(), options);

            if (root.TryGetProperty("source", out var src))
                Schema = JsonSerializer.Deserialize<SourceSchema>(src.GetRawText(), options);

            if (root.TryGetProperty("raw", out var raw))
                Raw = JsonSerializer.Deserialize<RawContent>(raw.GetRawText(), options);
        }
        catch { }
    }
}

public class NormalizedContent
{
    public string? Id { get; set; }
    public string? ExternalId { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? Summary { get; set; }
    public DateTime PublishedAt { get; set; }
    public string? Url { get; set; }
    public string? Author { get; set; }
    public string? Language { get; set; }
    public CategoryInfo? Category { get; set; }
}

public class CategoryInfo
{
    public string? Primary { get; set; }
    public List<string>? Secondary { get; set; }
}

public class SourceSchema
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? Url { get; set; }
    public bool RequiresSecret { get; set; }
}

public class RawContent
{
    public string? Format { get; set; }

    [JsonPropertyName("data")]
    public JsonElement? Data { get; set; }
}