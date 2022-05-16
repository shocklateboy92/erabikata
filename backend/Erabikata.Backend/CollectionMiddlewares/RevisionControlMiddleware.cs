using System;
using System.Threading.Tasks;
using Erabikata.Backend.CollectionManagers;
using Erabikata.Backend.Models.Actions;
using Erabikata.Models.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Erabikata.Backend.CollectionMiddlewares;

public class RevisionControlMiddleware : ICollectionMiddleware
{
    private readonly DatabaseInfoManager _databaseInfoManager;
    private readonly SubtitleProcessingSettings _settings;
    private readonly ILogger<RevisionControlMiddleware> _logger;

    public RevisionControlMiddleware(
        DatabaseInfoManager databaseInfoManager,
        IOptions<SubtitleProcessingSettings> settings,
        ILogger<RevisionControlMiddleware> logger
    )
    {
        _databaseInfoManager = databaseInfoManager;
        _logger = logger;
        _settings = settings.Value;
    }

    public async Task Execute(Activity activity, Func<Activity, Task> next)
    {
        switch (activity)
        {
            case BeginIngestion beginIngestion:
                var previousComment = await _databaseInfoManager.GetCurrentCommit();
                if (ShouldContinueIngestion(beginIngestion, previousComment))
                {
                    await next(beginIngestion);
                }

                break;
            default:
                await next(activity);
                break;
        }
    }

    private bool ShouldContinueIngestion(BeginIngestion beginIngestion, string previousComment)
    {
        if (beginIngestion.ForceFullIngestion)
        {
            _logger.LogInformation("Doing ingestion because it was forced by parameter.");
            return true;
        }

        if (previousComment != beginIngestion.StartCommit)
        {
            _logger.LogInformation(
                "Skipping ingestion because the previous commit ({PrevCommit})"
                    + " does not match start commit ({StartCommit})",
                previousComment,
                beginIngestion.StartCommit
            );
            return false;
        }

        if (_settings.AllowIngestionOfSameCommit)
        {
            _logger.LogInformation(
                "Not comparing begin and end commit because it was disabled in settings"
            );
            return true;
        }

        if (beginIngestion.StartCommit != beginIngestion.EndCommit)
        {
            _logger.LogInformation(
                "Skipping ingestion because start commit ({StartCommit})"
                    + " is the same as end commit ({EndCommit})",
                beginIngestion.StartCommit,
                beginIngestion.EndCommit
            );
            return false;
        }

        _logger.LogInformation("Doing ingestion because no conditions to skip were met");
        return true;
    }
}
