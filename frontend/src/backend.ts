import {
    ActionsExecuteApiResponse,
    api as generatedApi
} from './backend-rtk.generated';
import { DisableStyle, EnableStyle } from './backend.generated';

const api = generatedApi
    .enhanceEndpoints({
        addEntityTypes: [
            'WordOccurrences',
            'Dialog',
            'EngDialog',
            'ActiveStyle'
        ],
        endpoints: {
            actionsExecute: {
                invalidates: (_, { activity }) => {
                    switch (activity.activityType) {
                        case 'EnableStyle':
                        case 'DisableStyle':
                            return ['EngDialog', 'ActiveStyle'];
                        default:
                            return [];
                    }
                }
            },
            engSubsActiveStylesFor: {
                provides: ['ActiveStyle']
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
                EnableStyle | DisableStyle
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
