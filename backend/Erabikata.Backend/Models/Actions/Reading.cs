namespace Erabikata.Backend.Models.Actions;

public record LearnReading(int WordId) : Activity;

public record UnLearnReading(int WordId) : Activity;
