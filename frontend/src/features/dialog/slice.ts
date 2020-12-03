import { AsyncThunk, createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';
import { DialogInfo, NowPlayingInfo, SubsClient } from 'backend.generated';
import { selectBaseUrl } from 'features/backendSelection';
import { wordContextFetchSucceeded } from 'features/wordContext';

interface IDialogState {
    order: {
        [key: string]: number[] | undefined;
    };
    content: {
        [key: string]: undefined | { [key: number]: DialogInfo | undefined };
    };
}

export interface IDialogId {
    episode: string;
    time: number;
}

export const fetchDialogById: AsyncThunk<
    NowPlayingInfo,
    IDialogId & { count?: number },
    { state: RootState }
> = createAsyncThunk(
    'dialog/byEpisode',
    ({ episode, time, count }, { getState }) =>
        // Making this request unconditionally, because we don't know if there are
        // other dialog between the ones we have in the cache.
        new SubsClient(selectBaseUrl(getState)).index(episode, time, count ?? 3)
);

const dialogListReducer = (
    state: IDialogState,
    episodeId: string,
    dialog: DialogInfo[]
) => ({
    content: {
        ...state.content,
        [episodeId]: {
            ...state.content[episodeId],
            ...Object.fromEntries(dialog.map((d) => [d.startTime, d]))
        }
    },
    order: {
        ...state.order,
        [episodeId]: [
            ...new Set([
                ...(state.order[episodeId] || []),
                ...dialog.map((d) => d.startTime)
            ])
        ].sort((a, b) => a - b)
    }
});

const dialogSlice = createSlice({
    name: 'dialog',
    initialState: { content: {}, order: {} } as IDialogState,
    reducers: {},
    extraReducers: (builder) =>
        builder
            .addCase(fetchDialogById.fulfilled, (state, { payload }) =>
                dialogListReducer(state, payload.episodeId, payload.dialog)
            )
            .addCase(
                wordContextFetchSucceeded,
                (state, { payload: { occurrences } }) => {
                    for (const occurence of occurrences) {
                        if (!state.content[occurence.episodeId]) {
                            state.content[occurence.episodeId] = {};
                        }
                        state.content[occurence.episodeId]![
                            occurence.text.startTime
                        ] = occurence.text;

                        // TODO: Add times to the orders arrays
                    }
                }
            )
});

export const selectDialogContent = (
    episodeId: string,
    time: number,
    state: RootState
) => state.dialog.content[episodeId]?.[time];

export const selectNearbyDialog = (
    episodeId: string,
    time: number,
    count: number,
    state: RootState
) => selectNearbyValues(state.dialog.order[episodeId], time, count);

export const selectNearbyValues = (
    array: number[] | undefined,
    value: number,
    count: number
) => {
    const index = array?.findIndex((d) => d > value);
    if (index !== undefined && index >= 0) {
        return array!.slice(Math.max(0, index - count), index + count);
    }

    return [];
};

export const dialogReducer = dialogSlice.reducer;
