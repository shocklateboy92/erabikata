using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Erabikata.Backend.CollectionManagers;
using Erabikata.Backend.Models.Output;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Erabikata.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EngSubsController : ControllerBase
    {
        private readonly EngSubCollectionManager _engSubCollectionManager;
        private readonly StyleFilterCollectionManager _styleFilterCollection;

        public EngSubsController(
            EngSubCollectionManager engSubCollectionManager,
            StyleFilterCollectionManager styleFilterCollection)
        {
            _engSubCollectionManager = engSubCollectionManager;
            _styleFilterCollection = styleFilterCollection;
        }

        public async Task<ActionResult<EngSubsResponse>> Index(
            string episodeId,
            double timeStamp,
            int count = 3)
        {
            if (!int.TryParse(episodeId, out var episode))
            {
                return BadRequest();
            }

            var styles = await _styleFilterCollection.GetActiveStylesFor(episode);
            var subs = await _engSubCollectionManager.GetNearestSubs(
                episode,
                timeStamp,
                count,
                styles
            );
            return new EngSubsResponse
            {
                Dialog = subs.Adapt<IEnumerable<EngSubsResponse.Sentence>>()
            };
        }

        [Route("[action]/{showId}")]
        public async Task<StylesOfResponse> StylesOf(int showId)
        {
            var showInfo = await _styleFilterCollection.GetByShowId(showId);
            if (showInfo == null)
            {
                return new StylesOfResponse(
                    showId,
                    Array.Empty<AggregateSortByCountResult<string>>(),
                    Array.Empty<string>()
                );
            }

            return new StylesOfResponse(
                showId,
                await _engSubCollectionManager.GetAllStylesOf(showInfo.ForEpisodes),
                showInfo.EnabledStyles
            );
        }

        public record StylesOfResponse(
            int ShowId,
            IEnumerable<AggregateSortByCountResult<string>> AllStyles,
            IEnumerable<string> EnabledStyles);
    }
}