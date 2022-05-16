using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Erabikata.Models.Configuration;
using Microsoft.Extensions.Options;

namespace Erabikata.Backend.DataProviders;

public class SeedDataProvider
{
    private readonly SubtitleProcessingSettings _settings;

    public SeedDataProvider(IOptions<SubtitleProcessingSettings> settings)
    {
        _settings = settings.Value;
    }

    // TODO: Inline this below

    public IReadOnlyCollection<string> GetAllFiles()
    {
        return Directory
            .EnumerateFiles(_settings.Input.RootDirectory, "*", SearchOption.AllDirectories)
            .ToList();
    }

    public static async Task<T> DeserializeFile<T>(string path)
    {
        await using var file = File.OpenRead(path);
        var results = await JsonSerializer.DeserializeAsync<T>(
            file,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
        );
        return results!;
    }

    public static bool IsPathForEpisode(string path, string type, int epNum)
    {
        return path.EndsWith($"{type}/{epNum:00}.ass") || path.EndsWith($"{type}/{epNum:00}.srt");
    }

    public record ShowContentFile(int Id, string Path);
}
