import {
    ActionsExecuteApiResponse,
    api as generatedApi
} from './backend-rtk.generated';
import {
    DisableStyle,
    EnableStyle,
    LearnReading,
    SendToAnki,
    SyncAnki,
    UnLearnReading
} from './backend.generated';

const api = generatedApi
    .enhanceEndpoints({
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
            }
        }
    })
    .injectEndpoints({
        endpoints: (build) => ({
            executeAction: build.mutation<
                ActionsExecuteApiResponse,
                | EnableStyle
                | DisableStyle
                | SendToAnki
                | SyncAnki
                | LearnReading
                | UnLearnReading
            >({
                query: (queryArg) => ({
                    url: `/api/Actions/execute`,
                    method: 'POST',
                    body: queryArg
                }),
                invalidatesTags: (_response, _error, { activityType }) => {
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
            })
        })
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
