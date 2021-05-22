import { RootState } from 'app/rootReducer';
import { isKana, selectIsFuriganaHiddenForWords } from 'features/furigana';
import {
    selectNearestDialogContent,
    selectSelectedEpisodeTime,
    selectSelectedWords
} from 'features/selectedWord';

export const selectSentenceTimeToSend = (state: RootState) =>
    state.anki.sentence ?? selectSelectedEpisodeTime(state);

export const selectMeaningTimeToSend = (state: RootState) =>
    state.anki.meaning ?? selectSelectedEpisodeTime(state);

export const selectImageTimeToSend = (state: RootState) =>
    state.anki.image ?? selectSelectedEpisodeTime(state);

export const selectWordIdToSend = (state: RootState) =>
    state.anki.word?.id ?? selectSelectedWords(state)[0];

export const selectSentenceTextToSend = (state: RootState) => {
    const wordId = selectWordIdToSend(state);
    const sentenceTime = selectSentenceTimeToSend(state);
    if (!sentenceTime) {
        return;
    }

    const dialog = selectNearestDialogContent(state, sentenceTime);
    return dialog?.words
        .map((line) =>
            line
                .map((word) =>
                    word.definitionIds.includes(wordId) ||
                    isKana(word.displayText) ||
                    selectIsFuriganaHiddenForWords(state, word.definitionIds) ||
                    word.displayText === word.reading ||
                    !word.reading
                        ? word.displayText
                        : ` ${word.displayText}[${word.reading}]`
                )
                .join('')
        )
        .join('\n');
};
