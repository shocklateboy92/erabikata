using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Erabikata.Backend.Models.Configuration;

[DataContract]
public class VideoInputSettings
{
    [Required]
    public string RootDirectory { get; set; } = string.Empty;

    [Required]
    public string ImageCacheDir { get; set; } = string.Empty;
}