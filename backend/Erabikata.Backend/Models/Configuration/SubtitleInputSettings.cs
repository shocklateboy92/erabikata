using System.Runtime.Serialization;

namespace Erabikata.Backend.Models.Configuration;

[DataContract]
public class SubtitleProcessingSettings
{
    public SubtitleProcessingInputSettings Input { get; set; } = new();

    public bool AllowIngestionOfSameCommit { get; set; }
}