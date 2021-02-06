using System.Linq;

namespace Erabikata.Backend.Extensions
{
    public static class StringExtensions
    {
        public static int ParseId(this string key) => int.Parse(key.Split('/').Last());
    }
}