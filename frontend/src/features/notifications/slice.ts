import { createSlice, PayloadAction } from '@reduxjs/toolkit';

interface INotification {
    id: number;
    title?: string;
    text: string;
}

interface INotifactionState {
    nextId: number;
    active: number[];
    content: {
        [key: number]: INotification | undefined;
    };
}

const initialState: INotifactionState = {
    content: {},
    active: [],
    nextId: 0
};

const slice = createSlice({
    name: 'notifications',
    initialState,
    reducers: {
        notification: (
            state,
            { payload }: PayloadAction<{ title?: string; text: string }>
        ) => ({
            nextId: state.nextId + 1,
            active: [...state.active, state.nextId],
            content: {
                ...state.content,
                [state.nextId]: {
                    id: state.nextId,
                    ...payload
                }
            }
        }),
        notificationDeactivation: (
            state,
            { payload }: PayloadAction<number>
        ) => ({ ...state, active: state.active.filter((id) => id !== payload) })
    }
});

export const { notification, notificationDeactivation } = slice.actions;
export const notificationReducer = slice.reducer;
