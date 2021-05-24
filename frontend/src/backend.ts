import {
    ActionsExecuteApiResponse,
    api as generatedApi
} from './backend-rtk.generated';
import {
    DisableStyle,
    EnableStyle,
    SendToAnki,
    SyncAnki
} from './backend.generated';

const api = generatedApi
    .enhanceEndpoints({
        addEntityTypes: [
            'WordOccurrences',
            'Dialog',
            'EngDialog',
            'KnownWords',
            'ActiveStyle'
        ],
        endpoints: {
            engSubsActiveStylesFor: {
                provides: ['ActiveStyle']
            },
            wordsKnown: {
                provides: ['KnownWords']
            },
            engSubsIndex: {
                provides: ['EngDialog']
            },
            wordsOccurrences: {
                provides: (response) => [
                    {
                        type: 'WordOccurrences',
                        id: response.wordId
                    }
                ]
            }
        }
    })
    .injectEndpoints({
        endpoints: (build) => ({
            executeAction: build.mutation<
                ActionsExecuteApiResponse,
                EnableStyle | DisableStyle | SendToAnki | SyncAnki
            >({
                query: (queryArg) => ({
                    url: `/api/Actions/execute`,
                    method: 'POST',
                    body: queryArg
                }),
                invalidates: (_, { activityType }) => {
                    switch (activityType) {
                        case 'EnableStyle':
                        case 'DisableStyle':
                            return ['EngDialog', 'ActiveStyle'];
                        case 'SyncAnki':
                            return ['KnownWords'];
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
    useWordsKnownQuery,
    useEngSubsActiveStylesForQuery,
    useEngSubsByStyleNameQuery,
    useEngSubsStylesOfQuery
} = api;

export const apiEndpoints = api.endpoints;
export const apiReducer = api.reducer;
export const apiMiddleware = api.middleware;
