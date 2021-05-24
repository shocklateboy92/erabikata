import { AsyncThunk, createAsyncThunk } from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';
import { AppDispatch } from 'app/store';
import { apiEndpoints, useExecuteActionMutation } from 'backend';
import { SendToAnki, SyncAnki } from 'backend.generated';
import { notification } from 'features/notifications';
import { ankiSendCompletion } from './ankiSlice';
import {
    selectImageTimeToSend,
    selectMeaningTextToSend,
    selectSentenceLinkToSend,
    selectSentenceTextToSend,
    selectWordDefinitionToSend,
    selectWordTagsToSend
} from './selectors';

export const syncAnkiActivity = (): SyncAnki => ({ activityType: 'SyncAnki' });

export const sendToAnki: AsyncThunk<
    void,
    void,
    { state: RootState; dispatch: AppDispatch }
> = createAsyncThunk('sendToAnki', async (_, { getState, dispatch }) => {
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
    const toSkip = state.anki.word.definitions;

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
        primaryWordMeaning: word.english
            .filter((_, index) => !toSkip[index])
            .map((meaning) => meaning.senses.join('\n'))
            .join('\n\n'),
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
});

const throwError = (error: Error) => {
    throw error;
};
