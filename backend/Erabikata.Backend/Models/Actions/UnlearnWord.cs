using System.Runtime.Serialization;

namespace Erabikata.Backend.Models.Actions
{
    [DataContract]
    public class UnlearnWord : Activity
    {
        public UnlearnWord(string activityType, string baseForm) : base(activityType)
        {
            BaseForm = baseForm;
        }

        [DataMember] public string BaseForm { get; set; }
    }
}