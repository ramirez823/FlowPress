using FlowPress.Models;
namespace FlowPress.Services.Interfaces;

public interface INewsAggregatorService
{
    Task<List<SourceItemViewModel>> GetLatestNewsAsync(int totalCount = 12);
}