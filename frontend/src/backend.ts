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
        }
    }
});

export const { useWordsOccurrencesQuery, useSubsByIdStringQuery } = api;
export const apiReducer = api.reducer;
