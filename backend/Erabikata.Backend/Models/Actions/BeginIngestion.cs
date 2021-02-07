using System.Runtime.Serialization;

namespace Erabikata.Backend.Models.Actions
{
    public record BeginIngestion : Activity
    {
        public BeginIngestion(string startCommit, string endCommit)
        {
            StartCommit = startCommit;
            EndCommit = endCommit;
        }

        [DataMember] public string StartCommit { get; set; }

        [DataMember] public string EndCommit { get; set; }
    }
}