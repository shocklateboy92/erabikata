using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using SubtitlesParser.Classes.Parsers;

namespace srt_parser
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Directory.EnumerateFileSystemEntries(
                    Directory.GetCurrentDirectory(),
                    "*.srt",
                    SearchOption.AllDirectories
                )
                .AsParallel()
                .ForAll(
                    file =>
                    {
                        try
                        {
                            var lines = new SubParser().ParseStream(File.OpenRead(file))
                                .Select(
                                    item => new
                                    {
                                        startTime = TimeSpan.FromMilliseconds(item.StartTime).TotalSeconds,
                                        endTime = TimeSpan.FromMilliseconds(item.StartTime).TotalSeconds,
                                        tokenized = new[] {string.Join("\n", item.Lines)}
                                    }
                                );

                            var outDir = Path.GetDirectoryName(file) + Path.DirectorySeparatorChar +
                                         "subs";
                            Directory.CreateDirectory(outDir);

                            var outPath = outDir + Path.DirectorySeparatorChar +
                                          Path.ChangeExtension(Path.GetFileName(file), ".json");
                            using var writer = new StreamWriter(outPath);
                            new JsonSerializer().Serialize(
                                writer,
                                lines.ToArray()
                            );
                        }
                        catch (Exception e)
                        {
                            Console.Error.WriteLine($"Failed to process '{file}':");
                            Console.Error.WriteLine(e);
                        }
                    }
                );
        }
    }
}