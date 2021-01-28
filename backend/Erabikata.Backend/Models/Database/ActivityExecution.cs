using System.Runtime.Serialization;
using Erabikata.Backend.Models.Actions;
using MongoDB.Bson;

namespace Erabikata.Backend.Models.Database
{
    [DataContract]
    public class ActivityExecution
    {
        public ActivityExecution(ObjectId id, Activity activity)
        {
            Id = id;
            Activity = activity;
        }

        [DataMember]
        public ObjectId Id { get; set; }

        [DataMember]
        public Activity Activity { get; set; }
    }
}