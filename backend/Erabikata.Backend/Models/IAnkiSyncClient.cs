using System.Threading.Tasks;
using Refit;

namespace Erabikata.Backend.Models
{
    public interface IAnkiSyncClient
    {
        [Post("")]
        public Task<Response<long[]>> FindNotes([Body] FindNotesAction action);

        record BaseAction(string Action, object Params, int Version = 6);

        record FindNotesAction(string Query) : BaseAction("findNotes", new {Query});

        record Response<T>(T Result, string? Error);
    }
}