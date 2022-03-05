import { api as generatedApi } from './backend-rtk.generated';

const api = generatedApi.enhanceEndpoints({
    addTagTypes: [
        'WordOccurrences',
        'Dialog',
        'EngDialog',
        'KnownWords',
        'KnownReadings',
        'ActiveStyle'
    ],
    endpoints: {
        engSubsActiveStylesFor: {
            providesTags: ['ActiveStyle']
        },
        wordsKnown: {
            providesTags: ['KnownWords']
        },
        wordsNotes: { providesTags: ['KnownWords'] },
        engSubsIndex: {
            providesTags: ['EngDialog']
        },
        wordsOccurrences: {
            providesTags: (response) => [
                {
                    type: 'WordOccurrences',
                    id: response?.wordId
                }
            ]
        },
        wordsUnknownRanks: {
            providesTags: ['KnownWords']
        },
        actionsExecute: {
            invalidatesTags: (
                _response,
                _error,
                { activity: { activityType } }
            ) => {
                switch (activityType) {
                    case 'EnableStyle':
                    case 'DisableStyle':
                        return ['EngDialog', 'ActiveStyle'];
                    case 'SyncAnki':
                        return ['KnownWords'];
                    case 'LearnReading':
                    case 'UnLearnReading':
                        return ['KnownReadings'];
                    default:
                        return [];
                }
            }
        }
    }
});
export const {
    useWordsOccurrencesQuery,
    useSubsByIdQuery,
    useWordsRanked2Query,
    useWordsSearchQuery,
    useEpisodeIndexQuery,
    useEngSubsIndexQuery,
    useEngSubsShowIdOfQuery,
    useActionsExecuteMutation,
    useWordsUnknownRanksQuery,
    useWordsKnownQuery,
    useWordsNotesQuery,
    useEngSubsActiveStylesForQuery,
    useEngSubsByStyleNameQuery,
    useEngSubsStylesOfQuery
} = api;

export const apiEndpoints = api.endpoints;
export const apiReducer = api.reducer;
export const apiMiddleware = api.middleware;
