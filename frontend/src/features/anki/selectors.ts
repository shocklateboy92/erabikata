import { RootState } from 'app/rootReducer';
import { apiEndpoints } from 'backend';
import { isKana, selectIsFuriganaHiddenForWords } from 'features/furigana';
import {
    selectNearestDialogContent,
    selectSelectedEpisodeTime,
    selectSelectedWords
} from 'features/selectedWord';
import { generateDialogLink } from 'routing/linkGen';

export const selectSentenceTimeToSend = (state: RootState) =>
    state.anki.sentence ?? selectSelectedEpisodeTime(state);

export const selectMeaningTimeToSend = (state: RootState) => {
    const time = state.anki.meaning ?? selectSelectedEpisodeTime(state);
    if (time) {
        return { ...time, count: 0 };
    }
};

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

export const selectMeaningTextToSend = (state: RootState) => {
    const args = selectMeaningTimeToSend(state);
    if (args) {
        const { data } = apiEndpoints.engSubsIndex.select(args)(state);
        return data?.dialog[0].text.join('\n');
    }
};

export const selectSentenceLinkToSend = (state: RootState) => {
    const wordId = selectWordIdToSend(state);
    const sentenceTime = selectSentenceTimeToSend(state);
    if (!sentenceTime) {
        return;
    }

    return (
        'https://erabikata3.apps.lasath.org' +
        generateDialogLink(
            sentenceTime.episodeId,
            sentenceTime.time,
            wordId ? [wordId] : []
        )
    );
};
