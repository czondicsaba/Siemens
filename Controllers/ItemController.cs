using Microsoft.AspNetCore.Mvc;
using Siemens.Internship2026.GradeBook.Interfaces;
using Siemens.Internship2026.GradeBook.Models;

namespace Siemens.Internship2026.GradeBook.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ItemController : ControllerBase
{
    private readonly ILoggingService _logger;
    private readonly IStatisticsService _statisticsService;
    private readonly IItemReader _reader;
    private readonly IGradeFilterService _gradeFilterService;
    public ItemController(
        IItemReader reader,
        ILoggingService logger,
        IStatisticsService statisticsService,
        IGradeFilterService gradeFilterService)
    {
        _reader = reader;
        _logger = logger;
        _statisticsService = statisticsService;
        _gradeFilterService = gradeFilterService;
    }
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        _logger.Log("GET api/item called");
        var items = await _reader.GetAllAsync();
        var statistics = _statisticsService.CalculateStatistics(items);
        _logger.Log($"Returning {statistics.TotalCount} items, average value: {statistics.AverageValue}");
        return Ok(new
        {
            Data = items,
            Statistics = statistics
        });
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        _logger.Log($"GET api/item/{id} called");
        if (id <= 0)
        {
            _logger.Log($"Invalid id: {id}");
            return BadRequest("Id must be a positive integer.");
        }
        var item = await _reader.GetByIdAsync(id);
        if (item == null)
        {
            _logger.Log($"Item {id} not found");
            return NotFound($"Item with Id {id} was not found.");
        }
        return Ok(item);
    }
    [HttpGet("passing/first/{count}")]
    public async Task<IActionResult> GetFirstNPassingGrades(int count)
    {
        _logger.Log($"GET api/item/passing/first/{count} called");
        if (count <= 0)
        {
            _logger.Log($"Invalid count parameter: {count}");
            return BadRequest("Count must be a positive integer greater than 0.");
        }
        var filteredItems = await _gradeFilterService.GetFirstNPassingGradesFromRepositoryAsync(count);

        if (!filteredItems.Any())
        {
            _logger.Log($"No passing grades found for count={count}");
            return NotFound($"No passing grades found. Make sure there are active grades >= 5.");
        }
        var statistics = _statisticsService.CalculateStatistics(filteredItems);
        _logger.Log($"Returning {filteredItems.Count()} first passing grades out of requested {count}");
        return Ok(new
        {
            RequestedCount = count,
            ReturnedCount = filteredItems.Count(),
            Data = filteredItems,
            Statistics = statistics,
            FilterCriteria = new
            {
                IsActive = true,
                MinimumGrade = 5,
                Message = $"First {count} active grades with value >= 5"
            }
        });
    }
    [HttpGet("passing/first/{count}/filter")]
    public async Task<IActionResult> GetFirstNPassingGradesWithFilter(
        int count,
        [FromQuery] decimal? minGrade = null)
    {
        _logger.Log($"GET api/item/passing/first/{count}/filter called with minGrade={minGrade}");
        if (count <= 0)
        {
            return BadRequest("Count must be a positive integer greater than 0.");
        }
        var allItems = await _reader.GetAllAsync();
        var threshold = minGrade ?? 5;
        var filteredItems = allItems
            .Where(i => i.IsActive && i.Value >= threshold)
            .Take(count)
            .ToList();
        if (!filteredItems.Any())
        {
            return NotFound($"No active grades found with value >= {threshold}");
        }
        var statistics = _statisticsService.CalculateStatistics(filteredItems);
        return Ok(new
        {
            RequestedCount = count,
            ReturnedCount = filteredItems.Count,
            MinimumGradeThreshold = threshold,
            Data = filteredItems,
            Statistics = statistics
        });
    }
    [HttpGet("statistics/passing/first/{count}")]
    public async Task<IActionResult> GetStatisticsForFirstNPassingGrades(int count)
    {
        _logger.Log($"GET api/item/statistics/passing/first/{count} called");
        if (count <= 0)
        {
            return BadRequest("Count must be a positive integer greater than 0.");
        }
        var filteredItems = await _gradeFilterService.GetFirstNPassingGradesFromRepositoryAsync(count);
        if (!filteredItems.Any())
        {
            return NotFound($"No passing grades found for count={count}");
        }
        var statistics = _statisticsService.CalculateStatistics(filteredItems);
        return Ok(new
        {
            RequestedCount = count,
            ActualCount = filteredItems.Count(),
            Statistics = statistics,
            Timestamp = DateTime.UtcNow
        });
    }
}