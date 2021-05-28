import { mdiBookAlphabet } from '@mdi/js';
import Icon from '@mdi/react';
import { useTypedSelector } from 'app/hooks';
import React, { FC } from 'react';
import { selectWordDefinition } from './selectors';

const isAndroid = /(android)/i.test(navigator.userAgent);
const createJishoLink = (word: string) => `https://jisho.org/word/${word}`;
const createTakobotoLink = (wordId: number) =>
    [
        'intent:#Intent',
        'package=jp.takoboto',
        'action=jp.takoboto.WORD',
        'i.word=' + wordId,
        'S.browser_fallback_url=' +
            encodeURIComponent('http://takoboto.jp/?w=' + wordId),
        'end`'
    ].join(';');

export const ExternalDictionaryLink: FC<{ wordId: number }> = ({ wordId }) => {
    const headWord = useTypedSelector(
        (state) => selectWordDefinition(state, wordId)?.japanese[0].kanji
    );

    if (!headWord) {
        return null;
    }

    return (
        <a
            target="_blank"
            rel="noopener noreferrer"
            href={
                isAndroid
                    ? createTakobotoLink(wordId)
                    : createJishoLink(headWord)
            }
        >
            <Icon path={mdiBookAlphabet} />
        </a>
    );
};
