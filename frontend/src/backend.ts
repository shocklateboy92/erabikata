import { api as generatedApi } from './backend-rtk.generated';

const api = generatedApi.enhanceEndpoints({
    addEntityTypes: ['WordOccurrences', 'Dialog'],
    endpoints: {
        wordsOccurrences: {
            provides: (response) => [
                {
                    type: 'WordOccurrences',
                    id: response.wordId
                }
            ]
        }
    }
});

export const {
    useWordsOccurrencesQuery,
    useSubsByIdQuery,
    useWordsRanked2Query,
    useEpisodeIndexQuery,
    useEngSubsIndexQuery,
    useEngSubsByStyleNameQuery,
    useEngSubsStylesOfQuery
} = api;

export const apiEndpoints = api.endpoints;
export const apiReducer = api.reducer;
export const apiMiddleware = api.middleware;
