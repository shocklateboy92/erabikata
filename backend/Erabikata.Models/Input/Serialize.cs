using Newtonsoft.Json;

namespace Erabikata.Models.Input
{
    public static class Serialize
    {
        public static string ToJson(this Sentence[] self)
        {
            return JsonConvert.SerializeObject(self, Converter.Settings);
        }
    }
}