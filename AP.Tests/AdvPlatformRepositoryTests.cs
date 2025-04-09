using AP.Application.Interfaces;
using AP.Persistence.Repository;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AP.Tests;

public class AdvPlatformRepositoryTests
{
    private readonly Mock<ILogger<AdvPlatformRepository>> _mockLogger = new();
    private readonly IAdvPlatformRepository _repository;

    public AdvPlatformRepositoryTests()
    {
        _repository = new AdvPlatformRepository(_mockLogger.Object);
    }

    [Fact]
    public void FindPlatforms_ReturnsCorrectResults()
    {
        // Arrange
        var testData = new List<string>
        {
            "Яндекс.Директ:/ru",
            "Ревдинский рабочий:/ru/svrd/revda,/ru/svrd/pervik",
            "Газета уральских москвичей:/ru/msk,/ru/permobl,/ru/chelobl",
            "Крутая реклама:/ru/svrd"
        };

        _repository.LoadFromFile(testData);

        // Act & Assert
        Assert.Equivalent(new[] { "Яндекс.Директ" }, _repository.FindPlatforms("/ru"));
        Assert.Equivalent(new[] { "Яндекс.Директ", "Крутая реклама" }, _repository.FindPlatforms("/ru/svrd"));
        Assert.Equivalent(new[] { "Яндекс.Директ", "Ревдинский рабочий", "Крутая реклама" }, _repository.FindPlatforms("/ru/svrd/revda"));
        Assert.Empty(_repository.FindPlatforms(""));
        
    }

    [Fact]
    public void LoadFromFile_HandlesEmptyInput()
    {
        // Act
        _repository.LoadFromFile(new List<string>());

        // Assert
        Assert.Empty(_repository.FindPlatforms("/ru"));
    }

    [Fact]
    public void FindPlatforms_ReturnsEmpty_ForUnmatchedLocation()
    {
        // Arrange
        _repository.LoadFromFile(new List<string> { "Тест:/test" });

        // Act & Assert
        Assert.Empty(_repository.FindPlatforms("/non-existent"));
    }
}