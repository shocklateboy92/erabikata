using System.Runtime.Serialization;

namespace Erabikata.Backend.Models.Output;

[DataContract]
public record WordOccurrence(string EpisodeName, double Time, DialogInfo Text, string EpisodeId);
