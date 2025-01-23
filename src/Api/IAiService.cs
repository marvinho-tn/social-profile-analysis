namespace Api;

public interface IAiService
{
    Task<string> AnalyzeDataAsync(string data);
}