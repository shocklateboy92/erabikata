namespace Erabikata.Models.Input
{
    public class PagingInfo
    {
        public int Max { get; set; } = int.MaxValue;

        public int Skip { get; set; } = 0;
    }
}