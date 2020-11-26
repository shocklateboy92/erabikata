import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { updatePlayerList } from './api';

interface IHassState {
    players: { id: PlayerId; name?: string }[];
    selectedPlayer: PlayerId;
}
type PlayerId = string | null;

const initialState: IHassState = {
    players: [],
    selectedPlayer: null
};

const slice = createSlice({
    name: 'hass',
    initialState,
    reducers: {
        playerSelected: (state, { payload }: PayloadAction<PlayerId>) => ({
            ...state,
            selectedPlayer: payload
        })
    },
    extraReducers: (builder) =>
        builder.addCase(updatePlayerList.fulfilled, (state, { payload }) => ({
            ...state,
            players: [{ id: null, name: 'None' }, ...payload]
        }))
});

export const { playerSelected } = slice.actions;

export const hassReducer = slice.reducer;
