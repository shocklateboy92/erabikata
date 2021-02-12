using Erabikata.Backend.Models.Input.Generated;

namespace Erabikata.Backend.Models.Actions
{
    public record DictionaryIngestion(JMdict Dictionary) : Activity;
}