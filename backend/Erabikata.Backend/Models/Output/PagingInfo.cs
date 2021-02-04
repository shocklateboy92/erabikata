namespace Erabikata.Backend.Models.Output
{
    public class PagingInfo
    {
        public int Max { get; set; } = int.MaxValue;

        public int Skip { get; set; } = 0;
    }
}