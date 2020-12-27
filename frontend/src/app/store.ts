import { configureStore, ThunkAction, Action } from '@reduxjs/toolkit';
import { useDispatch } from 'react-redux';

import rootReducer, { RootState } from './rootReducer';

const store = configureStore({
    reducer: rootReducer,
    middleware: (getDefaultMiddleware) =>
        getDefaultMiddleware({ immutableCheck: false })
});

// TODO: figure out how CRA deals with HMR
// if (process.env.NODE_ENV === 'development' && module.hot) {
//   module.hot.accept('./rootReducer', () => {
//     const newRootReducer = require('./rootReducer').default
//     store.replaceReducer(newRootReducer)
//   })
// }

export type AppDispatch = typeof store.dispatch;
export type AppThunk = ThunkAction<void, RootState, unknown, Action<string>>;

export const useAppDispatch: () => AppDispatch = useDispatch;

export default store;
