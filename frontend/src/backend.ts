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
        subsById: {
            provides: (response) =>
                response.map((d) => ({ type: 'Dialog', id: d.text.id }))
        }
    }
});

export const { useWordsOccurrencesQuery, useSubsByIdQuery } = api;
export const apiReducer = api.reducer;
