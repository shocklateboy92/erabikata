import '@reduxjs/toolkit';
import { PayloadAction } from '@reduxjs/toolkit';

interface ISpinnerTopState {
    requests: {
        [key: string]: boolean | undefined;
    };
}

const initalState: ISpinnerTopState = { requests: {} };

export const spinnerTopReducer = (
    state = initalState,
    action: PayloadAction<
        unknown,
        string,
        // They don't export this type anywhere out of redux toolkit
        { requestId: string } | null,
        never
    >
): ISpinnerTopState => {
    if (action.meta?.requestId) {
        const {
            [action.meta.requestId]: request,
            ...otherRequests
        } = state.requests;
        if (request) {
            return { ...state, requests: otherRequests };
        } else {
            return {
                ...state,
                requests: { [action.meta.requestId]: true, ...state.requests }
            };
        }
    }

    return state;
};
