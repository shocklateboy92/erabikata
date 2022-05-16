using System.Runtime.Serialization;

namespace Erabikata.Backend.Models.Actions;

[DataContract]
public record LearnWord : Activity
{
    public LearnWord(string baseForm)
    {
        BaseForm = baseForm;
    }

    [DataMember]
    public string BaseForm { get; set; }
}
