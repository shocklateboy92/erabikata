import { AsyncThunk } from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';
import { WordClient, WordInfo } from 'backend.generated';
import { createApiCallThunk } from 'features/auth/api';
import { useLocation } from 'react-router-dom';

interface IWordContextArgs {
    baseForm?: string;
    onlyPartsOfSpeech?: string[];
    includeEpisode?: string | null;
    includeTime?: number | null;

    pagingInfo?: {
        max?: number;
        skip?: number;
    };
}

export const fetchWordIfNeeded: AsyncThunk<
    WordInfo,
    IWordContextArgs,
    { state: RootState }
> = createApiCallThunk(
    WordClient,
    'fetchWordContext',
    (
        client,
        {
            baseForm,
            pagingInfo,
            onlyPartsOfSpeech,
            includeEpisode,
            includeTime
        },
        analyzer
    ) => {
        return client.index(
            // This is checked for null by the `condition` below
            baseForm!,
            onlyPartsOfSpeech,
            includeEpisode,
            includeTime,
            pagingInfo?.max ?? 0,
            pagingInfo?.skip,
            analyzer
        );
    },
    {
        condition: ({ baseForm, pagingInfo }, { getState }) => {
            // Whitespace can be a valid word, but it won't pass .net model validation
            if (!baseForm || baseForm?.trim().length === 0) {
                return false;
            }

            const { wordContexts } = getState();
            const context = wordContexts.byId[baseForm];
            if (context) {
                const alreadyFetchedCount = context.occurrences.length;

                // If we already have more than we need, skip
                if (alreadyFetchedCount >= (pagingInfo?.max ?? 0)) {
                    return false;
                }

                // If we already have all there is, skip
                if (alreadyFetchedCount === context.totalOccurrences) {
                    return false;
                }
            }

            return true;
        }
    }
);

export const fetchFullWordIfNeeded = (
    baseForm: string | undefined,
    location: ReturnType<typeof useLocation>
) => {
    if (!baseForm) {
        return;
    }

    const params = new URLSearchParams(location.search);
    var time = params.get('includeTime');
    return fetchWordIfNeeded({
        baseForm,
        includeEpisode: params.get('includeEpisode'),
        includeTime: time ? parseFloat(time) : undefined,
        pagingInfo: { max: 200, skip: 0 }
    });
};
