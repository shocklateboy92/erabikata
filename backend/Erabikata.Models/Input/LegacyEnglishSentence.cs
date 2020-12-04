using Erabikata.Models.Input.V2;
using Newtonsoft.Json;

namespace Erabikata.Models.Input
{
#nullable enable
    public class LegacyEnglishSentence : InputSentence
    {
        public LegacyEnglishSentence(string text, double time, string style, long? size) : base(
            new[] {text},
            time,
            style,
            size
        )
        {
        }
    }
}