import { AsyncThunk, createAsyncThunk } from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';
import { apiEndpoints } from 'backend';
import { SendToAnki } from 'backend.generated';
import { notification } from 'features/notifications';
import {
    selectImageTimeToSend,
    selectMeaningTextToSend,
    selectSentenceLinkToSend,
    selectSentenceTextToSend,
    selectWordDefinitionToSend,
    selectWordTagsToSend
} from './selectors';

export const sendToAnki: AsyncThunk<
    void,
    void,
    { state: RootState }
> = createAsyncThunk('sendToAnki', async (_, { getState, dispatch }) => {
    const state = getState();
    const word = selectWordDefinitionToSend(state);
    if (!word) {
        return dispatch(
            notification({
                title: 'Unable to send',
                text: 'Primary word not selected or definition unavailable'
            })
        );
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

    return dispatch(apiEndpoints.executeAction.initiate(activity));
});

const throwError = (error: Error) => {
    throw error;
};
