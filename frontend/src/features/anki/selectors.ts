import { RootState } from 'app/rootReducer';
import { apiEndpoints } from 'backend';
import { isKana, selectIsFuriganaHiddenForWords } from 'features/furigana';
import {
    selectNearestDialogContent,
    selectSelectedEpisodeTime,
    selectSelectedWords
} from 'features/selectedWord';
import { selectWordDefinition } from 'features/wordDefinition/selectors';
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

export const selectWordDefinitionToSend = (state: RootState) => {
    const wordId = selectWordIdToSend(state);
    if (wordId) {
        return selectWordDefinition(state, wordId);
    }
};

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

export const selectWordTagsToSend = (state: RootState) => {
    const definition = selectWordDefinitionToSend(state);
    if (!definition) {
        return;
    }
    const toSkip = state.anki.word.definitions;

    const uniq = new Set();
    for (const [index, meaning] of definition.english.entries()) {
        if (toSkip[index]?.every((skip) => skip)) {
            continue;
        }

        for (const tag of meaning.tags) {
            uniq.add(tag);
        }
    }

    return [...uniq.values()].join(', ');
};

export const selectWordMeaningTextToSend = (state: RootState) => {
    const toSkip = state.anki.word.definitions;
    const word = selectWordDefinitionToSend(state);

    return word?.english
        .map((meaning, meaningIndex) =>
            meaning.senses
                .filter(
                    (_, senseIndex) => !(toSkip[meaningIndex] ?? [])[senseIndex]
                )
                .join('\n')
        )
        .filter((senses) => !!senses)
        .join('\n\n');
};
