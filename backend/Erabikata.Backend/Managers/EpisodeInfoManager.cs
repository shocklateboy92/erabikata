using System.Collections.Generic;
using System.IO;
using System.Linq;
using Erabikata.Models;
using Erabikata.Models.Configuration;
using Erabikata.Models.Output;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using Microsoft.Extensions.Options;

namespace Erabikata.Backend.Managers
{
    public class EpisodeInfoManager
    {
        private readonly SubtitleProcessingSettings _settings;

        private readonly IDictionary<string, EpisodeInfo> _cache =
            new Dictionary<string, EpisodeInfo>();

        private readonly VideoInputSettings _videoInputSettings;

        public EpisodeInfoManager(
            IOptions<SubtitleProcessingSettings> settings,
            IOptions<VideoInputSettings> videoInputSettings)
        {
            _videoInputSettings = videoInputSettings.Value;
            _settings = settings.Value;
        }

        public EpisodeInfo GetEpisodeInfo(
            Episode ep,
            bool clearCache = false,
            IUrlHelper? url = null)
        {
            if (clearCache)
            {
                _cache.Clear();
            }

            if (_cache.ContainsKey(ep.Title))
            {
                return _cache[ep.Title];
            }

            var file = Path.GetFileName(ep.Title);
            var number = int.Parse(_settings.Input.EpisodeNamePattern.Match(file).Groups[1].Value);
            var patternFile = Path.Join(
                Path.GetDirectoryName(ep.Title),
                _settings.Input.VideoMappingFileName
            );

            var pattern = GetPatternFrom(patternFile);
            var matcher = new Matcher();
            if (pattern != null)
            {
                matcher.AddInclude(string.Format(pattern, number));
            }

            var patternMatchingResult = matcher.Execute(
                new DirectoryInfoWrapper(new DirectoryInfo(_videoInputSettings.RootDirectory))
            );
            var targetFile = patternMatchingResult.HasMatches
                ? Path.GetFullPath(
                    patternMatchingResult.Files.First().Path,
                    _videoInputSettings.RootDirectory
                )
                : null;
            var engFile = patternMatchingResult.HasMatches
                ? Path.Join(
                    _videoInputSettings.EngSubsRootDirectory,
                    Path.ChangeExtension(patternMatchingResult.Files.First().Path, "json")
                )
                : null;

            var result = new EpisodeInfo
            {
                Number = number,
                Title = file.Remove(file.Length - 5),
                FileName = ep.Title,
                PatternFile = patternFile,
                Pattern = pattern,
                PatternGlob =
                    pattern != null
                        ? _videoInputSettings.RootDirectory + Path.DirectorySeparatorChar +
                          string.Format(pattern, number)
                        : null,
                VideoFile = targetFile,
                EngSubs = engFile,
                SubsLink = url?.Action(
                    "Index",
                    "Subs",
                    new {episode = ep.Title},
                    url.ActionContext.HttpContext.Request.Scheme
                )
            };

            _cache.Add(ep.Title, result);
            return result;
        }

        private static string? GetPatternFrom(string patternFile) =>
            File.Exists(patternFile) ? File.ReadAllText(patternFile).Trim() : null;
    }
}