using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Erabikata.Backend.Models.Actions
{
    public record IngestFiles : Activity
    {
        public IngestFiles(ICollection<string> filesInSeed)
        {
            FilesInSeed = filesInSeed;
        }

        [DataMember] public ICollection<string> FilesInSeed { get; }
    }
}