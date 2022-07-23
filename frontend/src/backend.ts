import { enhancedApi } from './backend.generated';

const api = enhancedApi.enhanceEndpoints({
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
            invalidatesTags: (_response, _error, { activity }) => {
                switch (activity.activityType) {
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
    useExecuteActionMutation,
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
