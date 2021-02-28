using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NJsonSchema.Converters;
using Refit;

namespace Erabikata.Backend.Models
{
    public interface IAnkiSyncClient
    {
        [Post("/")]
        public Task<AnkiResponse<long[]>> FindNotes([Body(buffered: true)] FindNotes action);
    }

    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public record AnkiAction(string Action, object Params, int Version = 6);

    public record FindNotes : AnkiAction
    {
        public FindNotes(string query) : base("findNotes", new {query})
        {
        }
    };

    public record AnkiResponse<T>(T Result, string? Error);
}