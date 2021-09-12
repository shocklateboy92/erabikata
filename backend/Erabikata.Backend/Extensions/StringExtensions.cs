using System.Linq;

namespace Erabikata.Backend.Extensions
{
    public static class StringExtensions
    {
        public static int ParseId(this string key)
        {
            return int.Parse(key.Split('/').Last());
        }
    }
}
