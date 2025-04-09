namespace AP.Application.Interfaces;

public interface IAdvPlatformRepository
{
    void LoadFromFile(List<string> lines);
    List<string> FindPlatforms(string location);
}