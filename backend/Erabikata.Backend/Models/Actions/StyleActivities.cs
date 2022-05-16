namespace Erabikata.Backend.Models.Actions;

public record EnableStyle(int ShowId, string StyleName) : Activity;

public record DisableStyle(int ShowId, string StyleName) : Activity;
