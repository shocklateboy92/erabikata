import { createSlice, PayloadAction } from '@reduxjs/toolkit';

const initialState: { open: { [key: string]: boolean | undefined } } = {
    open: {}
};
const slice = createSlice({
    name: 'drawer',
    initialState,
    reducers: {
        drawerToggleRequest: (
            { open },
            { payload }: PayloadAction<string>
        ) => ({ open: { ...open, [payload]: !open[payload] } })
    }
});

export const { drawerToggleRequest } = slice.actions;
export const drawerReducer = slice.reducer;
