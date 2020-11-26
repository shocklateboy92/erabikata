import {
    createAsyncThunk,
    createEntityAdapter,
    createSlice,
    EntityState
} from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';
import { EnglishSentence, EngSubsClient } from 'backend.generated';
import { selectNearbyValues } from 'features/dialog/slice';

interface IEnglishEpisode {
    id: string;
    dialog: EntityState<EnglishSentence>;
}

const timeAdapter = createEntityAdapter<EnglishSentence>({
    selectId: (s) => s.time,
    sortComparer: (a, b) => a.time - b.time
});
const episodeAdapter = createEntityAdapter<IEnglishEpisode>();

export const fetchEnglishDialog = createAsyncThunk(
    'englishDialog',
    (args: [episodeId: string, time: number]) =>
        // We fetch unconditionally because there may be missing gaps in dialog
        new EngSubsClient().index(...args)
);

const slice = createSlice({
    name: 'englishDialog',
    initialState: episodeAdapter.getInitialState(),
    reducers: {},
    extraReducers: (builder) =>
        builder.addCase(fetchEnglishDialog.fulfilled, (state, action) => {
            const [id] = action.meta.arg;

            episodeAdapter.upsertOne(state, {
                id,
                dialog: timeAdapter.upsertMany(
                    state.entities[id]?.dialog ?? timeAdapter.getInitialState(),
                    action.payload.dialog
                )
            });
        })
});

const timeSelectors = timeAdapter.getSelectors();
const episodeSelectors = episodeAdapter.getSelectors();
export const selectNearbyEnglishDialog = (
    state: RootState,
    episodeId: string,
    time: number
) =>
    selectNearbyValues(
        episodeSelectors.selectById(state.engDialog, episodeId)?.dialog
            .ids as number[],
        time,
        2
    );

export const selectEnglishDialogContent = (
    state: RootState,
    episodeId: string,
    time: number
) => {
    const episode = episodeSelectors.selectById(state.engDialog, episodeId);
    if (episode?.dialog) {
        return timeSelectors.selectById(episode.dialog, time);
    }
};
export const engDialogReducer = slice.reducer;
