import { createSlice, PayloadAction } from '@reduxjs/toolkit';

export interface IEpisodeTime {
    episodeId: string;
    time: number;
}

interface IAnkiState {
    sentence?: IEpisodeTime;
    meaning?: IEpisodeTime;
    word: { id?: number; definitions: boolean[] };
    image?: IEpisodeTime;
}

const initialState: IAnkiState = { word: { definitions: [] } };

const slice = createSlice({
    name: 'anki',
    initialState,
    reducers: {
        wordMeaningCheckToggle: (state, { payload }: PayloadAction<number>) => {
            state.word.definitions[payload] = !state.word.definitions[payload];
        },
        ankiSendCompletion: () => initialState
    }
});

export const ankiReducer = slice.reducer;
export const { wordMeaningCheckToggle, ankiSendCompletion } = slice.actions;
