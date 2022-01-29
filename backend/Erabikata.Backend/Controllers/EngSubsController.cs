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
            StyleFilterCollectionManager styleFilterCollection
        )
        {
            _engSubCollectionManager = engSubCollectionManager;
            _styleFilterCollection = styleFilterCollection;
        }

        public async Task<ActionResult<EngSubsResponse>> Index(
            string episodeId,
            double time,
            int count = 3
        )
        {
            if (!int.TryParse(episodeId, out var episode))
            {
                return BadRequest();
            }

            var styles = await _styleFilterCollection.GetActiveStylesFor(episode);
            var subs = await _engSubCollectionManager.GetNearestSubs(episode, time, count, styles);
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

        [Route("[action]/{showId}")]
        public async Task<IEnumerable<string>> ActiveStylesFor(int showId)
        {
            var show = await _styleFilterCollection.GetByShowId(showId);
            return show?.EnabledStyles ?? Array.Empty<string>();
        }

        [Route("[action]/{episodeId}")]
        public async Task<ActionResult<int?>> ShowIdOf(string episodeId)
        {
            if (!int.TryParse(episodeId, out var parsedEpisodeId))
            {
                return BadRequest();
            }

            return await _styleFilterCollection.GetShowIdOf(parsedEpisodeId);
        }

        [Route("[action]")]
        public async Task<EngSubsResponse> ByStyleName(
            int showId,
            string styleName,
            [FromQuery] PagingInfo pagingInfo
        )
        {
            var filter = await _styleFilterCollection.GetByShowId(showId);
            if (filter == null)
            {
                return new EngSubsResponse();
            }

            var subs = await _engSubCollectionManager.GetByStyleName(
                filter.ForEpisodes,
                styleName,
                pagingInfo
            );
            return new EngSubsResponse
            {
                Dialog = subs.Adapt<IEnumerable<EngSubsResponse.Sentence>>()
            };
        }
    }
}
