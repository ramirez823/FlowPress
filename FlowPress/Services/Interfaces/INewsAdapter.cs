using FlowPress.Models;
namespace FlowPress.Services.Interfaces;

public interface INewsAdapter
{
    string SourceName { get; }
    string SourceUrl { get; }
    Task<List<SourceItemViewModel>> FetchAsync(string apiKey, int count);
}