import '@reduxjs/toolkit';

interface ISpinnerTopState {
    activeRequests: number;
    requests: {
        [key: string]: boolean | undefined;
    };
}

const initalState: ISpinnerTopState = { activeRequests: 0, requests: {} };

interface OsrAction<T> {
    meta?: {
        osr?: 'start' | 'end';
        requestId?: string;
    };
    payload: T;
}

export const spinnerTopReducer = (
    state = initalState,
    action: OsrAction<unknown>
): ISpinnerTopState => {
    // Match requests created with `createAsyncThunk()`
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

    // Match requests created by my ghetto version
    switch (action?.meta?.osr) {
        case 'start':
            return { ...state, activeRequests: state.activeRequests + 1 };
        case 'end':
            return { ...state, activeRequests: state.activeRequests - 1 };
        default:
            return state;
    }
};

export const osrEnd = <T>(payload: T): OsrAction<T> => ({
    meta: { osr: 'end' },
    payload
});
export const osrStart = <T>(payload: T): OsrAction<T> => ({
    meta: { osr: 'start' },
    payload
});
