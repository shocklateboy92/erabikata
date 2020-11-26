import { combineReducers } from '@reduxjs/toolkit';
import { wordContextsReducer } from '../features/wordContext';
import { selectedWordReducer } from 'features/selectedWord/slice';
import { nowPlayingReducer } from 'features/nowPlaying';
import { spinnerTopReducer } from 'features/spinnerTop';
import { wordDefinitionReducer } from 'features/wordDefinition';
import { furiganaReducer } from 'features/furigana/slice';
import { dialogReducer } from 'features/dialog/slice';
import { wordRanksReducer } from 'features/rankedWords/slice';
import { engDialogReducer } from 'features/engDialog/slice';
import { hassReducer } from 'features/hass';

const rootReducer = combineReducers({
    wordContexts: wordContextsReducer,
    spinnerTop: spinnerTopReducer,
    selectedWord: selectedWordReducer,
    wordDefinitions: wordDefinitionReducer,
    nowPlaying: nowPlayingReducer,
    dialog: dialogReducer,
    furigana: furiganaReducer,
    wordRanks: wordRanksReducer,
    engDialog: engDialogReducer,
    hass: hassReducer
});

export type RootState = ReturnType<typeof rootReducer>;

export default rootReducer;
