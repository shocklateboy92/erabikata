import { AsyncThunk, createAsyncThunk } from '@reduxjs/toolkit';
import { RootState } from '../../app/rootReducer';
import { notification } from 'features/notifications';

export const encodeSelectionParams = (
    episode: string,
    time: number,
    words: number[]
) => {
    const params = new URLSearchParams();
    params.set('episode', episode);
    params.set('time', time.toString());
    for (const word of words) {
        params.append('word', word.toString());
    }

    return params.toString();
};

export const shareSelectedWordDialog: AsyncThunk<
    string | undefined,
    never,
    { state: RootState }
> = createAsyncThunk(
    'shareSelectedWordDialog',
    async (_, { getState, dispatch }) => {
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

        const params = encodeSelectionParams(
            episode,
            sentenceTimestamp,
            wordIds
        );
        const text = `[${word}](${document.baseURI}/dialog?${params}) #Japanese`;

        await navigator.share({ text });
        return text;
    }
);
