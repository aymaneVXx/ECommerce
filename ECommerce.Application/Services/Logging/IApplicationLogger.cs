namespace ECommerce.Application.Services.Logging;

public interface IApplicationLogger<T>
{
    void LogInformation(string message);
    void LogWarning(string message);
    void LogError(string message);
    void LogError(Exception ex, string message);
}
