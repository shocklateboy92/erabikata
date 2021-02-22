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
        },
        subsByIdString: {
            provides: (response) =>
                response.map((d) => ({ type: 'Dialog', id: d.text.id }))
        },
        subsIndex: {
            provides: (response) =>
                response.dialog.map(({ id }) => ({ type: 'Dialog', id }))
        }
    }
});

export const {
    useWordsOccurrencesQuery,
    useSubsIndexQuery,
    useSubsByIdQuery,
    useWordsRanked2Query,
    useEpisodeIndexQuery,
} = api;

export const apiEndpoints = api.endpoints;
export const apiReducer = api.reducer;
export const apiMiddleware = api.middleware;
