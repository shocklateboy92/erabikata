using System.Runtime.Serialization;

namespace Erabikata.Backend.Models.Actions
{
    public record BeginIngestion : Activity
    {
        public BeginIngestion(string startCommit, string endCommit, bool forceFullIngestion = false)
        {
            StartCommit = startCommit;
            EndCommit = endCommit;
            ForceFullIngestion = forceFullIngestion;
        }

        [DataMember]
        public string StartCommit { get; set; }

        [DataMember]
        public string EndCommit { get; set; }

        [DataMember]
        public bool ForceFullIngestion { get; set; }
    }
}
