using System.Collections.Generic;
using System.Threading.Tasks;
using Erabikata.Backend.Models.Actions;
using Mapster;
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

        [Post("/")]
        public Task<AnkiResponse<long?>> AddNote([Body(buffered: true)] AddNoteAnkiAction action);
    }

    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public record AnkiAction(string Action, object Params, int Version = 6);
    //public record AnkiAction<TParams>(string Action, TParams Params, int Version = 6);

    public record AddNoteAnkiAction : AnkiAction
    {
        public AddNoteAnkiAction(SendToAnki activity)
            : base(
                "addNote",
                new
                {
                    note = new AddNoteParams(
                        DeckName: "Takoboto",
                        ModelName: "Jap Sentences 2",
                        Fields: activity.Adapt<Dictionary<string, string>>()
                    )
                }
            )
        {}

        [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
        public record AddNoteParams(
            string DeckName,
            string ModelName,
            IDictionary<string, string> Fields
        );
    };

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
