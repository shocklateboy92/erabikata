using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Refit;

namespace Erabikata.Backend.Models
{
    public interface IAnkiSyncClient
    {
        [Post("/")]
        public Task<AnkiResponse<long[]>> FindNotes([Body(buffered: true)] AnkiAction action);

        [Post("/")]
        public Task<AnkiResponse<AnkiNote[]>> NotesInfo([Body(buffered: true)] AnkiAction action);
    }

    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public record AnkiAction(string Action, object Params, int Version = 6);

    public record AnkiNote(
        long NoteId,
        string ModelName,
        IReadOnlyCollection<string> Tags,
        IReadOnlyDictionary<string, AnkiNote.Field> Fields)
    {
        public record Field(string Value, int Order);
    };

    public record AnkiResponse<T>(T Result, string? Error);
}
