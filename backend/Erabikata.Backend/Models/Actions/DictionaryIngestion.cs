using System.Collections.Generic;
using System.Xml.Linq;
using Erabikata.Backend.Models.Database;

namespace Erabikata.Backend.Models.Actions
{
    public record DictionaryIngestion(IReadOnlyCollection<WordInfo> Dictionary) : Activity;
}