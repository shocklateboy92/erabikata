using System.Runtime.Serialization;

namespace Erabikata.Backend.Models.Actions
{
    public record EndIngestion : Activity
    {
        public EndIngestion(string startCommit, string endCommit)
        {
            StartCommit = startCommit;
            EndCommit = endCommit;
        }

        [DataMember]
        public string StartCommit { get; set; }

        [DataMember]
        public string EndCommit { get; set; }
    }
}
