using Microsoft.Extensions.Logging;

namespace Erabikata.Backend.Extensions
{
    public static class LoggerExtensions
    {
        public static void LogInformationString<T>(this ILogger<T> logger, string log)
        {
            logger.LogInformation("{Log}", log);
        }
    }
}