import { AsyncThunk, createAsyncThunk } from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';
import { WordClient, WordInfo } from 'backend.generated';
import { selectAnalyzer, selectBaseUrl } from 'features/backendSelection';
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
> = createAsyncThunk(
    'fetchWordContext',
    (
        {
            baseForm,
            pagingInfo,
            onlyPartsOfSpeech,
            includeEpisode,
            includeTime
        },
        { getState }
    ) => {
        const state = getState();
        return new WordClient(selectBaseUrl(state)).index(
            // This is checked for null by the `condition` below
            baseForm!,
            onlyPartsOfSpeech,
            includeEpisode,
            includeTime,
            pagingInfo?.max ?? 0,
            pagingInfo?.skip,
            selectAnalyzer(state)
        );
    },
    {
        condition: ({ baseForm, pagingInfo }, { getState }) => {
            // Whitespace can be a valid word, but it won't pass .net model validation
            if (!baseForm || baseForm?.trim().length === 0) {
                return false;
            }

            const { wordContexts } = getState();
            if (pagingInfo?.max) {
                // This is kind of a ghetto way of busting the cached word summary
                // when navigating to a word details page.
                // TODO: Replace with proper paging/scrolling support.
                return (
                    (wordContexts.byId[baseForm]?.occurrences.length ?? 0) === 0
                );
            }

            return !wordContexts.byId[baseForm];
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
