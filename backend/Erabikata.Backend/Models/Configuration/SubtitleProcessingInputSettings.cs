using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Erabikata.Backend.Models.Configuration;

[DataContract]
public class SubtitleProcessingInputSettings
{
    [Required]
    public string RootDirectory { get; set; } = string.Empty;
}