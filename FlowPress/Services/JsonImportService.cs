using FlowPress.Models;
using FlowPress.Models.ImportExport;
using FlowPress.Repositories.Interfaces;
using System.Text.Json;

namespace FlowPress.Services;

public class JsonImportService
{
    private readonly ISourceRepository _sourceRepository;
    private readonly ISourceItemRepository _sourceItemRepository;

    public JsonImportService(
        ISourceRepository sourceRepository,
        ISourceItemRepository sourceItemRepository)
    {
        _sourceRepository = sourceRepository;
        _sourceItemRepository = sourceItemRepository;
    }

    public async Task<string> ImportAsync(string jsonContent)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var data = JsonSerializer.Deserialize<IngestRoot>(jsonContent, options);

        if (data == null)
            throw new Exception("JSON inválido.");

        if (data.SchemaVersion != "edu.univ.ingest.v1")
            throw new Exception("Schema inválido.");

        // Buscar fuente
        var allSources = await _sourceRepository.GetAllAsync();
        var existingSource = allSources.FirstOrDefault(s => s.Url == data.Source.Url);

        IngestModels sourceToUse;

        if (existingSource == null)
        {
            sourceToUse = new IngestModels
            {
                Url = data.Source.Url,
                Name = data.Source.Name,
                Description = $"Importado desde {data.SchemaVersion}",
                ComponentType = data.Source.Type,
                RequiresSecret = data.Source.RequiresSecret
            };

            await _sourceRepository.AddAsync(sourceToUse);
        }
        else
        {
            sourceToUse = existingSource;
        }

        // Guardar JSON
        var allItems = await _sourceItemRepository.GetAllAsync();

        var duplicateItem = allItems.FirstOrDefault(i => i.Json == jsonContent);

        if (duplicateItem != null)
            throw new Exception("Ese JSON ya fue importado anteriormente.");

        var sourceItem = new SourceItem
        {
            SourceId = sourceToUse.Id,
            Json = jsonContent,
            CreatedAt = DateTime.UtcNow
        };

        await _sourceItemRepository.AddAsync(sourceItem);

        return "Importación exitosa";
    }
}