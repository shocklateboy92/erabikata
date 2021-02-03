using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Erabikata.Backend.CollectionManagers;
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

        // TODO: Inline this below
        public IEnumerable<string> GetShowMetadataFilesI()
        {
            // TODO: Figure out how to make this async
            return Directory.EnumerateFiles(
                _settings.Input.RootDirectory,
                "show-metadata.json",
                SearchOption.AllDirectories
            );
        }


        public async Task<ICollection<ShowContentFile>[]> GetShowMetadataFiles(
            string dataType,
            string fileType)
        {
            return await Task.WhenAll(
                GetShowMetadataFilesI()
                    .Select(
                        async metadataFilePath =>
                        {
                            var metadata = await DeserializeFile<ShowInfo>(metadataFilePath);
                            return metadata.Episodes[0].Select(
                                    (episode, index) => new ShowContentFile(
                                        Id: int.Parse(episode.Key.Split('/').Last()),
                                        Path: GetDataPath(
                                            dataType,
                                            metadataFilePath,
                                            index,
                                            fileType
                                        )
                                    )
                                )
                                .ToList();
                        }
                    )
            );
        }

        public static string GetDataPath(
            string dataType,
            string metadataFile,
            int index,
            string fileType) =>
            Path.Combine(metadataFile, $"../{dataType}/{index + 1:00}.{fileType}");

        public record ShowContentFile(int Id, string Path);

        public static async Task<T> DeserializeFile<T>(string path)
        {
            await using var file = File.OpenRead(path);
            var results = await JsonSerializer.DeserializeAsync<T>(
                file,
                new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase}
            );
            return results!;
        }
    }
}