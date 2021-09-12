using System.Collections.Generic;
using System.Threading.Tasks;
using Erabikata.Backend.CollectionManagers;
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
        public Task<AnkiResponse<long?>> Execute([Body(buffered: true)] AddNoteAnkiAction action);

        [Post("/")]
        public Task<AnkiResponse<object?>> Execute([Body(buffered: true)] SyncAnkiAction action);
    }

    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public record AnkiAction(string Action, object Params, int Version = 6);

    public record SyncAnkiAction() : AnkiAction("sync", new {  });

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
                        Fields: activity.Adapt<Dictionary<string, string>>(),
                        Picture: new[]
                        {
                            new PictureInfo(
                                Url: $"https://erabikata3.apps.lasath.org/api/image/{activity.Image.EpisodeId}/{activity.Image.Time}?includeSubs={activity.Image.IncludeSubs}",
                                Filename: $"erabikata_{activity.Image.EpisodeId}_{activity.Image.Time}.png",
                                Fields: new[] { "Image" }
                            )
                        }
                    )
                }
            ) { }

        [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
        public record AddNoteParams(
            string DeckName,
            string ModelName,
            IDictionary<string, string> Fields,
            IEnumerable<PictureInfo> Picture
        );

        [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
        public record PictureInfo(string Url, string Filename, string[] Fields);
    };

    public record AnkiNote(
        long NoteId,
        string ModelName,
        IReadOnlyCollection<string> Tags,
        IReadOnlyDictionary<string, AnkiNote.Field> Fields
    ) {
        public record Field(string Value, int Order);
    };

    public record AnkiResponse<T>(T Result, string? Error)
    {
        public T Unwrap()
        {
            if (!string.IsNullOrEmpty(this.Error))
            {
                throw new AnkiConnectException(this.Error);
            }

            return this.Result;
        }
    }
}
