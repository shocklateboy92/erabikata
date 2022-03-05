using System.Collections.Generic;
using System.Threading.Tasks;
using Erabikata.Backend.CollectionManagers;
using Microsoft.AspNetCore.Mvc;

namespace Erabikata.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlternateIdsController : ControllerBase
{
    private readonly AlternateIdCollectionManager _alternateIdCollection;

    public AlternateIdsController(AlternateIdCollectionManager alternateIdCollection)
    {
        _alternateIdCollection = alternateIdCollection;
    }

    [HttpGet]
    [Route("[action]")]
    public Task<IReadOnlyDictionary<string, string>> Map()
    {
        return _alternateIdCollection.GetAllEntries();
    }
}
