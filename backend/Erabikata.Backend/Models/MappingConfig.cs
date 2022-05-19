using Erabikata.Backend.Models.Database;
using Erabikata.Backend.Models.Output;
using Mapster;

namespace Erabikata.Backend.Models;

public class ModelsMappingConfigRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config
            .ForType<Dialog.Word, DialogInfo.WordRef>()
            .MapToConstructor(true)
            .MapWith(
                word =>
                    new DialogInfo.WordRef(
                        word.OriginalForm,
                        word.BaseForm,
                        word.Reading,
                        word.InfoIds
                    )
            );
    }
}
