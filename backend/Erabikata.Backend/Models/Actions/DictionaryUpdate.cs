using System.Runtime.Serialization;

namespace Erabikata.Backend.Models.Actions
{
    public record DictionaryUpdate(string SourceUrl) : Activity;
}