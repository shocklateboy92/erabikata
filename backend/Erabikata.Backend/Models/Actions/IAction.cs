using JsonSubTypes;
using Newtonsoft.Json;

namespace Erabikata.Backend.Models.Actions
{
    [JsonConverter(typeof(JsonSubtypes), "ActionType")]
    [JsonSubtypes.KnownSubTypeAttribute(typeof(LearnWord), nameof(LearnWord))]
    [JsonSubtypes.KnownSubTypeAttribute(typeof(UnlearnWord), nameof(UnlearnWord))]
    public class Action
    {
        public Action(string actionType)
        {
            ActionType = actionType;
        }

        public string ActionType { get; }
    }
}