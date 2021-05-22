import { AsyncThunk, createAsyncThunk } from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';
import { apiEndpoints } from 'backend';
import { SendToAnki } from 'backend.generated';
import { selectSentenceTextToSend } from './selectors';

export const sendToAnki: AsyncThunk<
    void,
    void,
    { state: RootState }
> = createAsyncThunk('sendToAnki', async (_, { getState, dispatch }) => {
    const state = getState();
    const activity: SendToAnki = {
        activityType: 'SendToAnki',
        text: selectSentenceTextToSend(state)!,
        link: '',
        image: '',
        notes: '',
        meaning: '',
        primaryWord: '',
        primaryWordMeaning: '',
        primaryWordReading: ''
    };

    return dispatch(apiEndpoints.executeAction.initiate(activity));
});
