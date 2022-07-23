import { AsyncThunk, createAsyncThunk } from '@reduxjs/toolkit';
import { RootState } from '../../app/rootReducer';
import { notification } from 'features/notifications';
import { useTypedSelector } from 'app/hooks';
import { selectSelectedWord } from './selectors';

export const encodeSelectionParams = (
    episode: string | undefined,
    time: number | undefined,
    words: number[],
    baseForm?: string
) => {
    const params = new URLSearchParams();
    if (episode) {
        params.set('episode', episode);
    }
    if (time) {
        params.set('time', time.toString());
    }

    // Base form must be first, or will be ignored by slice
    if (baseForm) {
        params.append('word', baseForm);
    }
    for (const word of words) {
        params.append('word', word.toString());
    }

    return params.toString();
};

export const useEncodedSelectionParams = () => {
    const { wordIds, episode, sentenceTimestamp } =
        useTypedSelector(selectSelectedWord);
    return encodeSelectionParams(episode, sentenceTimestamp, wordIds);
};

export const shareSelectedWordDialog = createAsyncThunk<
    string | undefined,
    never,
    { state: RootState }
>('shareSelectedWordDialog', async (_, { getState, dispatch }) => {
    const {
        selectedWord: { episode, sentenceTimestamp, wordIds },
        wordDefinitions
    } = getState();
    if (!(episode && sentenceTimestamp && wordIds)) {
        return;
    }

    const word = wordDefinitions.byId[wordIds[0]]?.japanese[0].kanji;
    if (!word) {
        dispatch(
            notification({
                title: 'Unable to share',
                text: `Primary word ${wordIds[0]} had no definition fetched.`
            })
        );
    }

    const params = encodeSelectionParams(episode, sentenceTimestamp, wordIds);
    const text = `[${word}](${window.location.origin}/ui/dialog?${params}) #Japanese`;

    await navigator.share({ text });
    return text;
});
