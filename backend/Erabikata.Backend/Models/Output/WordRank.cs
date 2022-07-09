namespace Erabikata.Backend.Models.Output;

public record WordRank(int Id, long? Rank);

public record WordRankInfo(int Id, int Rank, ulong Count, string Text);
