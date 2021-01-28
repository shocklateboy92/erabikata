using System.Runtime.Serialization;

namespace Erabikata.Backend.Models.Actions
{
    public class Activity
    {
        protected Activity(string activityType)
        {
            ActivityType = activityType;
        }

        [DataMember] public string ActivityType { get; set; }
    }
}