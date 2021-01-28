using System.Runtime.Serialization;

namespace Erabikata.Backend.Models.Actions
{
    [DataContract]
    public class LearnWord : Action
    {
        public LearnWord(string actionType, string baseForm) : base(actionType)
        {
            BaseForm = baseForm;
        }

        [DataMember] public string BaseForm { get; }
    }
}