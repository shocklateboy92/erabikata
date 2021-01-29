using System.Runtime.Serialization;

namespace Erabikata.Backend.Models.Actions
{
    public class BeginIngestion : Activity
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