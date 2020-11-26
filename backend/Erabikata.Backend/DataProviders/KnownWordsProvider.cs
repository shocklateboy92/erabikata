using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Erabikata.Models;
using Newtonsoft.Json;
using Octokit;
using Octokit.Internal;

namespace Erabikata.Backend.DataProviders
{
    public class KnownWordsProvider
    {
        private readonly HttpClient _httpClient;

        public KnownWordsProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IReadOnlyCollection<string>> GetKnownWords()
        {
            var client = new GitHubClient(new ProductHeaderValue("shocklateboy92-sturdy-guide"));
            var contents = await client.Repository.Content.GetAllContents(
                "shocklateboy92",
                "known-words",
                "known-words.txt"
            );

            var content = contents.First().Content;
            return content.Split("\n")
                .Where(line => !(line.StartsWith('#') || string.IsNullOrWhiteSpace(line)))
                .ToHashSet();
        }
    }
}