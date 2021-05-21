import { createSlice } from '@reduxjs/toolkit';

interface ITimeStamp {
    episodeId: string;
    time: number;
}

interface IAnkiState {
    sentence?: { id: string };
    meaning?: ITimeStamp;
    word?: { id: number; definitions?: number[] };
    image?: ITimeStamp;
}

const initialState: IAnkiState = {};

const slice = createSlice({ name: 'anki', initialState, reducers: {} });

export const ankiReducer = slice.reducer;
