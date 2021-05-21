import { combineReducers } from '@reduxjs/toolkit';
import { ankiReducer } from 'features/anki/ankiSlice';
import { authReducer } from 'features/auth/slice';
import { backendReducer } from 'features/backendSelection';
import { furiganaReducer } from 'features/furigana/slice';
import { hassReducer } from 'features/hass';
import { notificationReducer } from 'features/notifications';
import { selectedWordReducer } from 'features/selectedWord/slice';
import { spinnerTopReducer } from 'features/spinnerTop';
import { todoistReducer } from 'features/todoist/slice';
import { wakeLockReducer } from 'features/wakeLock';
import { wordDefinitionReducer } from 'features/wordDefinition';
import { apiReducer } from '../backend';

const rootReducer = combineReducers({
    spinnerTop: spinnerTopReducer,
    selectedWord: selectedWordReducer,
    wordDefinitions: wordDefinitionReducer,
    furigana: furiganaReducer,
    backend: backendReducer,
    todoist: todoistReducer,
    wakeLock: wakeLockReducer,
    auth: authReducer,
    notifications: notificationReducer,
    api: apiReducer,
    anki: ankiReducer,
    hass: hassReducer
});

export type RootState = ReturnType<typeof rootReducer>;

export default rootReducer;
