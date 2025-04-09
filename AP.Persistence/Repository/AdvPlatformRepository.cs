using AP.Application.Interfaces;
using AP.Domain;
using Microsoft.Extensions.Logging;

namespace AP.Persistence.Repository;

public class AdvPlatformRepository : IAdvPlatformRepository
{
    private readonly List<AdvPlatform> _platforms = new();
    private readonly Dictionary<string, List<string>> _locationIndex = new();
    private readonly ILogger<AdvPlatformRepository> _logger;

    public AdvPlatformRepository(ILogger<AdvPlatformRepository> logger)
    {
        _logger = logger;
    }
    
    /// <summary>
    /// Загрузка файла в оперативную память.
    /// </summary>
    public void LoadFromFile(List<string> lines)
    {
        _platforms.Clear();
        _locationIndex.Clear();

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || !line.Contains(":"))
            {
                _logger.LogDebug("Пропущена пустая или некорректная строка: {Line}", line);
                continue;
            }

            var parts = line.Split(":", 2);
            var name = parts[0].Trim();
            var locations = parts[1].Split(",", StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.Trim()).ToList();
            
            if (locations.Count == 0)
            {
                _logger.LogWarning("Платформа без локаций: {PlatformName}", name);
                continue;
            }

            var platform = new AdvPlatform { Name = name, Locations = locations };
            _platforms.Add(platform);
            
            foreach (var loc in locations)
            {
                if (!_locationIndex.ContainsKey(loc))
                    _locationIndex[loc] = new List<string>();
                _locationIndex[loc].Add(name);
            }
        }
    }

    /// <summary>
    /// Поиск платформы.
    /// </summary>
    public List<string> FindPlatforms(string location)
    {
        
        if (string.IsNullOrWhiteSpace(location))
        {
            _logger.LogWarning("Пустая локация для поиска");
            return new List<string>();
        }
        
        var result = new HashSet<string>();
        var current = location;
        
        _logger.LogDebug("Начало поиска платформ для локации: {Location}", location);
        
        while (!string.IsNullOrEmpty(current))
        {
            if (_locationIndex.TryGetValue(current, out var platforms))
            {
                foreach (var p in platforms)
                    result.Add(p);
            }
            
            var lastSlash = current.LastIndexOf('/');
            current = lastSlash >= 0 ? current[..lastSlash] : null;
        }
        
        return result.ToList();
    }
}