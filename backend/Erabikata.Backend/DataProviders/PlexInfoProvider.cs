using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading.Tasks;
using Erabikata.Models.Input.Plex;
using Microsoft.AspNetCore.WebUtilities;
using MediaTypeHeaderValue = Microsoft.Net.Http.Headers.MediaTypeHeaderValue;

namespace Erabikata.Backend.DataProviders
{
    public class PlexInfoProvider
    {
        private readonly HttpClient _httpClient;

        public PlexInfoProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Sessions> GetCurrentSessions(string plexToken)
        {
            var response = await _httpClient.SendAsync(
                new HttpRequestMessage(
                    HttpMethod.Get,
                    QueryHelpers.AddQueryString(
                        "https://plex.apps.lasath.org/status/sessions",
                        new Dictionary<string, string>() {{"X-Plex-Token", plexToken}}
                    )
                )
                {
                    Headers =
                    {
                        Accept =
                        {
                            new MediaTypeWithQualityHeaderValue(
                                MediaTypeNames.Application.Json
                            )
                        }
                    }
                }
            );

            response.EnsureSuccessStatusCode();

            return Sessions.FromJson(await response.Content.ReadAsStringAsync());
        }
    }
}