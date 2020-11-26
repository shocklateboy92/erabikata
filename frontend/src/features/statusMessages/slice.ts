import '@reduxjs/toolkit';
import { createSlice } from '@reduxjs/toolkit';

interface IStatusMessagesState {
    active: { text: string }[];
}

const initialState: IStatusMessagesState = { active: [] };

export const slice = createSlice({
    name: 'statusMessages',
    initialState,
    reducers: {}
});
