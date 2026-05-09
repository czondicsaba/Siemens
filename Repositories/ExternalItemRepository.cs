using System.Text.Json;
using Siemens.Internship2026.GradeBook.Dtos;
using Siemens.Internship2026.GradeBook.Interfaces;
using Siemens.Internship2026.GradeBook.Models;

namespace Siemens.Internship2026.GradeBook.Repositories;

public class ExternalItemRepository : IItemReader
{
    private readonly HttpClient _httpClient;
    private readonly ILoggingService _logger;
    private List<Item> _items = new();
    private DateTime _lastUpdated = DateTime.MinValue;
    private readonly SemaphoreSlim _refreshLock = new(1, 1);
    private bool _isInitialLoadComplete = false;
    private readonly string _dataEndpoint = "https://gist.githubusercontent.com/ArdeleanTudor/8ea407832cd9794960e0e6bbd1319f6e/raw/145b121103dd1";
    public DateTime LastUpdated => _lastUpdated;
    public bool IsDataLoaded => _items.Any() && _isInitialLoadComplete;
    public ExternalItemRepository(HttpClient httpClient, ILoggingService logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    public async Task<Item?> GetByIdAsync(int id)
    {
        await EnsureDataLoadedAsync();
        var item = _items.FirstOrDefault(i => i.Id == id && i.IsActive);
        _logger.Log($"GetByIdAsync({id}) returned {(item != null ? "item" : "null")}");
        return item;
    }
    public async Task<IEnumerable<Item>> GetAllAsync()
    {
        await EnsureDataLoadedAsync();
        var activeItems = _items.Where(i => i.IsActive).ToList();
        _logger.Log($"GetAllAsync returned {activeItems.Count} active items out of {_items.Count} total");
        return activeItems;
    }
    public async Task RefreshDataAsync()
    {
        await LoadDataFromExternalSource(forceRefresh: true);
    }
    private async Task EnsureDataLoadedAsync()
    {
        if (_isInitialLoadComplete && _items.Any())
            return;

        await LoadDataFromExternalSource(forceRefresh: false);
    }
    private async Task LoadDataFromExternalSource(bool forceRefresh)
    {
      
        if (!forceRefresh && _isInitialLoadComplete &&
            DateTime.UtcNow - _lastUpdated < TimeSpan.FromMinutes(30))
        {
            _logger.Log("Using cached data, last updated at {LastUpdated}", _lastUpdated);
            return;
        }

        await _refreshLock.WaitAsync();
        try
        {
            _logger.Log($"Fetching data from external API: {_dataEndpoint}");

            var response = await _httpClient.GetAsync(_dataEndpoint);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            _logger.Log($"Received JSON response length: {json.Length} characters");
            var externalResponse = JsonSerializer.Deserialize<ExternalItemsResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            if (externalResponse?.Items == null || !externalResponse.Items.Any())
            {
                _logger.Log("No items found in external data source");
                return;
            }
            var newItems = externalResponse.Items.Select(dto => new Item
            {
                Id = dto.Id,
                Value = dto.Value,
                IsActive = dto.IsActive
            }).ToList();
            _items = newItems;
            _lastUpdated = DateTime.UtcNow;
            _isInitialLoadComplete = true;
            _logger.Log($"Successfully loaded {_items.Count} items from external API");
            _logger.Log($"Active items: {_items.Count(i => i.IsActive)}, Inactive items: {_items.Count(i => !i.IsActive)}");
            var passingGrades = _items.Count(i => i.IsActive && i.Value >= 5);
            _logger.Log($"Passing grades (>=5): {passingGrades}");
        }
        catch (HttpRequestException ex)
        {
            _logger.Log($"HTTP error fetching external data: {ex.Message}");
            if (!_isInitialLoadComplete)
                throw new InvalidOperationException("Failed to load initial data from external source", ex);
        }
        catch (JsonException ex)
        {
            _logger.Log($"JSON deserialization error: {ex.Message}");
            if (!_isInitialLoadComplete)
                throw;
        }
        catch (Exception ex)
        {
            _logger.Log($"Unexpected error loading external data: {ex.Message}");
            if (!_isInitialLoadComplete)
                throw;
        }
        finally
        {
            _refreshLock.Release();
        }
    }
}