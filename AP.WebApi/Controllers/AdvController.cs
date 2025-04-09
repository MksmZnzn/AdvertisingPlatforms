using AP.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AP.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdvController : ControllerBase
{
    private readonly IAdvPlatformRepository _repository;

    public AdvController(IAdvPlatformRepository repository)
    {
        _repository = repository;
    }
    
    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [RequestSizeLimit(10_485_760)]//1GB
    [RequestFormLimits(MultipartBodyLengthLimit = 10_485_760)]
    public async Task<IActionResult> Load(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Файл не выбран или пуст.");

        var lines = new List<string>();

        using (var reader = new StreamReader(file.OpenReadStream()))
        {
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (!string.IsNullOrWhiteSpace(line))
                    lines.Add(line.Trim());
            }
        }

        _repository.LoadFromFile(lines);
        return Ok("Файл успешно загружен.");
    }
    
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Search([FromQuery] string? location)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(location))
                return BadRequest("Локация не задана.");

            if (!location.StartsWith("/"))
                return BadRequest("Локация должна начинаться с /");

            var result = _repository.FindPlatforms(location);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ошибка при поиске: {ex.Message}");
        }
    }
}