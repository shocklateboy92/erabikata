using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace srt_extractor
{
    public static class Program
    {
        private const string SubsRoot = "../anime-subs/watched";
        private const string EngSubsRoot = "../anime-subs/english";
        private const string VideoRoot = "/mnt/net/media";
        private const string VideoMappingName = "video_mapping.txt";

        private static readonly string[] TrackNames =
            {"[Doki] ASS (Anime)", "Chihiro (Without Insert KFx)", "Full Subtitles (FFF modified)", "UTW"};

        private static readonly string[] Languages = {"eng"};

        static async Task Main(string[] args)
        {
            var directories = Directory.GetDirectories(
                SubsRoot,
                "analyzed_subs",
                SearchOption.AllDirectories
            );
            var files = directories
                .SelectMany(dir => Directory.EnumerateFiles(dir, VideoMappingName))
                .ToArray();

            Console.WriteLine($"Found {files.Length} video mapping files.");

            foreach (var patternFile in files)
            {
                var pattern = (await File.ReadAllTextAsync(patternFile)).Trim();
                var entries = Directory.GetFiles(VideoRoot, pattern.Replace("{0:00}", "??"));

                foreach (var entry in entries)
                {
                    var outputPath = Path.ChangeExtension(
                        entry.Replace(VideoRoot, EngSubsRoot),
                        "ass"
                    );
                    var fileInfo = new FileInfo(outputPath);
                    if (fileInfo.Exists)
                    {
                        continue;
                    }

                    fileInfo.Directory?.Create();

                    var mediaInfo = await FFmpeg.GetMediaInfo(entry);
                    var tracks = mediaInfo.SubtitleStreams.Where(
                            stream => string.IsNullOrWhiteSpace(stream.Language) ||
                                      Languages.Contains(
                                          stream.Language,
                                          StringComparer.CurrentCultureIgnoreCase
                                      )
                        )
                        .ToList();

                    Console.WriteLine($"Got {tracks.Count} tracks for '{entry}'");
                    var track = tracks.Count switch
                    {
                        < 1 => throw new Exception(
                            $"Unable to determine sub track for file '{entry}:'\n\t{string.Join("\n\t", tracks.Select(stream => stream.Title))}"
                        ),
                        1 => tracks.Single(),
                        > 1 when tracks.Any(stream => TrackNames.Contains(stream.Title)) =>
                            tracks.Single(stream => TrackNames.Contains(stream.Title)),
                        _ => throw new Exception(
                            $"Unable to determine sub track for file '{entry}:'\n\t{string.Join("\n\t", tracks.Select(stream => $"{stream.Language}: {stream.Title}"))}"
                        ),
                    };

                    var conv = FFmpeg.Conversions.New()
                        .AddStream(track)
                        .SetOutput(outputPath)
                        .SetOutputFormat(Format.ass);
                    Console.WriteLine(conv.Build());
                    await conv.Start();
                }
            }
        }
    }
}
