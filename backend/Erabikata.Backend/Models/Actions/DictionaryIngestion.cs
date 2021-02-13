using System.Collections.Generic;
using System.Xml.Linq;
using Erabikata.Backend.Models.Database;

namespace Erabikata.Backend.Models.Actions
{
    public record DictionaryIngestion(IEnumerable<WordInfo> Dictionary) : Activity;
}