import { combineReducers } from '@reduxjs/toolkit';
import { authReducer } from 'features/auth/slice';
import { backendReducer } from 'features/backendSelection';
import { dialogReducer } from 'features/dialog/slice';
import { engDialogReducer } from 'features/engDialog/slice';
import { furiganaReducer } from 'features/furigana/slice';
import { hassReducer } from 'features/hass';
import { notificationReducer } from 'features/notifications';
import { wordRanksReducer } from 'features/rankedWords/slice';
import { selectedWordReducer } from 'features/selectedWord/slice';
import { spinnerTopReducer } from 'features/spinnerTop';
import { wakeLockReducer } from 'features/wakeLock';
import { wordDefinitionReducer } from 'features/wordDefinition';
import { wordContextsReducer } from '../features/wordContext';

const rootReducer = combineReducers({
    wordContexts: wordContextsReducer,
    spinnerTop: spinnerTopReducer,
    selectedWord: selectedWordReducer,
    wordDefinitions: wordDefinitionReducer,
    dialog: dialogReducer,
    furigana: furiganaReducer,
    wordRanks: wordRanksReducer,
    engDialog: engDialogReducer,
    backend: backendReducer,
    wakeLock: wakeLockReducer,
    auth: authReducer,
    notifications: notificationReducer,
    hass: hassReducer
});

export type RootState = ReturnType<typeof rootReducer>;

export default rootReducer;
