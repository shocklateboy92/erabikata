using System.Xml.Linq;

namespace Erabikata.Backend.Models.Actions
{
    public record DictionaryIngestion(XElement Dictionary) : Activity;
}