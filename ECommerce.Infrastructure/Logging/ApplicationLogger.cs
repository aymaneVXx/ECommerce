using ECommerce.Application.Services.Logging;
using Microsoft.Extensions.Logging;

namespace ECommerce.Infrastructure.Logging;

public class ApplicationLogger<T> : IApplicationLogger<T>
{
    private readonly ILogger<T> _logger;

    public ApplicationLogger(ILogger<T> logger)
    {
        _logger = logger;
    }

    public void LogInformation(string message) => _logger.LogInformation(message);

    public void LogWarning(string message) => _logger.LogWarning(message);

    public void LogError(string message) => _logger.LogError(message);

    public void LogError(Exception ex, string message) => _logger.LogError(ex, message);
}
