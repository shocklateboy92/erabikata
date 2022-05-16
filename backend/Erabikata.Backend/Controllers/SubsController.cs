using System.Linq;
using System.Threading.Tasks;
using Erabikata.Backend.CollectionManagers;
using Erabikata.Backend.Extensions;
using Erabikata.Models.Input;
using Erabikata.Models.Output;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Erabikata.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubsController : ControllerBase
{
    private readonly DialogCollectionManager _dialogCollection;
    private readonly PartOfSpeechFilterCollectionManager _partOfSpeechFilter;

    public SubsController(
        DialogCollectionManager dialogCollection,
        PartOfSpeechFilterCollectionManager partOfSpeechFilter
    )
    {
        _dialogCollection = dialogCollection;
        _partOfSpeechFilter = partOfSpeechFilter;
    }

    [Route("[action]/{id}")]
    public async Task<ActionResult<WordOccurrence>> ById(
        [BindRequired] Analyzer analyzer,
        string id
    )
    {
        var (dialogs, ignoredPartsOfSpeech) = await (
            _dialogCollection.GetByIds(analyzer.ToAnalyzerMode(), new[] { id }),
            _partOfSpeechFilter.GetIgnoredPartOfSpeech()
        );
        var dialog = dialogs.FirstOrDefault();
        if (dialog == null)
        {
            return NotFound($"Unable to find dialog with id '{id}'.");
        }

        var ignoreReadingMap = ignoredPartsOfSpeech.ToHashSet();

        return new WordOccurrence
        {
            EpisodeId = dialog.EpisodeId.ToString(),
            EpisodeName = dialog.EpisodeTitle,
            Time = dialog.Time,
            Text = new DialogInfo(
                dialog.Id.ToString(),
                dialog.Time,
                dialog.Lines
                    .Select(
                        list =>
                            list.Words
                                .Select(
                                    word =>
                                        new DialogInfo.WordRef(
                                            word.OriginalForm,
                                            word.BaseForm,
                                            word.PartOfSpeech.Any(ignoreReadingMap.Contains)
                                              ? string.Empty
                                              : word.Reading,
                                            word.InfoIds
                                        )
                                )
                                .ToArray()
                    )
                    .ToArray(),
                isSongLyric: dialog.ExcludeWhenRanking
            )
        };
    }
}
