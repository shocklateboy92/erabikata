using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Erabikata.Models.Configuration;
using Erabikata.Models.Input.V2;
using Microsoft.Extensions.Options;

namespace Erabikata.Backend.DataProviders
{
    public class SeedDataProvider
    {
        private readonly SubtitleProcessingSettings _settings;

        public SeedDataProvider(IOptions<SubtitleProcessingSettings> settings)
        {
            _settings = settings.Value;
        }
        
        public ICollection<string> GetShowMetadataFiles()
        {
            // TODO: Figure out how to make this async
            return Directory.EnumerateFiles(
                    _settings.Input.RootDirectory,
                    "show-metadata.json",
                    SearchOption.AllDirectories
                )
                .ToList();
        }

        public static string GetDataPath(string dataType, string metadataFile, int index) =>
            Path.Combine(metadataFile, $"../{dataType}/{index + 1:00}.json");
    }
}