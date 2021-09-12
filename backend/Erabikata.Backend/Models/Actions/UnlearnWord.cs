using System.Runtime.Serialization;

namespace Erabikata.Backend.Models.Actions
{
    [DataContract]
    public record UnlearnWord : Activity
    {
        public UnlearnWord(string baseForm)
        {
            BaseForm = baseForm;
        }

        [DataMember]
        public string BaseForm { get; set; }
    }
}
