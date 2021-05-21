import { createSlice } from '@reduxjs/toolkit';

interface IEpisodeTime {
    episodeId: string;
    time: number;
}

interface IAnkiState {
    sentence?: IEpisodeTime;
    meaning?: IEpisodeTime;
    word?: { id: number; definitions?: number[] };
    image?: IEpisodeTime;
}

const initialState: IAnkiState = {};

const slice = createSlice({ name: 'anki', initialState, reducers: {} });

export const ankiReducer = slice.reducer;
