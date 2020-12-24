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
                                        text = item.Lines,
                                        time = TimeSpan.FromMilliseconds(item.StartTime)
                                            .TotalSeconds,
                                        size = item.Lines.Aggregate(
                                            0,
                                            (size, line) => size + line.Length
                                        ),
                                        style = "Srt"
                                    }
                                );

                            var outPath = Path.ChangeExtension(file, ".json");
                            using var writer = new StreamWriter(outPath);
                            new JsonSerializer {Formatting = Formatting.Indented}.Serialize(
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