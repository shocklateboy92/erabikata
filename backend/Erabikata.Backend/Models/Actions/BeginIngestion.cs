using System.Runtime.Serialization;

namespace Erabikata.Backend.Models.Actions;

public record BeginIngestion(string EndCommit) : Activity;
