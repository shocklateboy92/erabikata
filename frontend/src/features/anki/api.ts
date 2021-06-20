import { AsyncThunk, createAsyncThunk } from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';
import { AppDispatch } from 'app/store';
import { apiEndpoints } from 'backend';
import { SendToAnki, SyncAnki } from 'backend.generated';
import { notification } from 'features/notifications';
import { ankiSendCompletion } from './ankiSlice';
import {
    selectImageTimeToSend,
    selectMeaningTextToSend,
    selectSentenceLinkToSend,
    selectSentenceTextToSend,
    selectWordDefinitionToSend,
    selectWordMeaningTextToSend,
    selectWordTagsToSend
} from './selectors';

export const syncAnkiActivity = (): SyncAnki => ({ activityType: 'SyncAnki' });

export const sendToAnki: AsyncThunk<
    void,
    void,
    { state: RootState; dispatch: AppDispatch }
> = createAsyncThunk('sendToAnki', async (_, { getState, dispatch }) => {
    if (Notification.permission !== 'granted') {
        // Have to do this early so it's still "in response to a user action"
        Notification.requestPermission();
    }

    const state = getState();
    const word = selectWordDefinitionToSend(state);
    if (!word) {
        dispatch(
            notification({
                title: 'Unable to send',
                text: 'Primary word not selected or definition unavailable'
            })
        );
        return;
    }
    const [{ reading, kanji }] = word.japanese;

    const activity: SendToAnki = {
        activityType: 'SendToAnki',
        text: selectSentenceTextToSend(state)!,
        link: selectSentenceLinkToSend(state)!,
        image: { ...selectImageTimeToSend(state)!, includeSubs: true },
        notes: selectWordTagsToSend(state)!,
        meaning: selectMeaningTextToSend(state)!,
        primaryWord:
            kanji ??
            reading ??
            throwError(new Error(`Word ${word.id} had no kanji or reading`)),
        primaryWordMeaning: selectWordMeaningTextToSend(state)!,
        primaryWordReading: reading ?? ''
    };

    const { data, error } = await dispatch(
        apiEndpoints.executeAction.initiate(activity)
    );
    if (!data) {
        dispatch(
            notification({
                title: 'Failed to create note',
                text: JSON.stringify(error)
            })
        );
        return;
    }

    dispatch(ankiSendCompletion());

    await dispatch(apiEndpoints.executeAction.initiate(syncAnkiActivity()));

    if (document.hidden) {
        const worker = await navigator.serviceWorker.getRegistration();
        worker?.showNotification('Successfully sent note to Anki');
    }
});

const throwError = (error: Error) => {
    throw error;
};
