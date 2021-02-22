using Microsoft.AspNetCore.Mvc;

namespace Erabikata.Backend.Models.Output
{
    public class PagingInfo
    {
        [FromQuery]
        public int Max { get; set; } = int.MaxValue;

        [FromQuery]
        public int Skip { get; set; } = 0;
    }
}