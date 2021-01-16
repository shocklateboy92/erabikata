import { RootState } from 'app/rootReducer';
import store, { AppThunk } from 'app/store';
import { isKana, toggleWordFurigana } from 'features/furigana';
import { notification } from 'features/notifications';
import {
    selectSelectedDialog,
    selectSelectedEnglishDialog,
    selectSelectedWord
} from 'features/selectedWord';
import { selectDefinitionById } from 'features/wordDefinition';

const copyAction = (
    selector: (state: RootState) => string | undefined,
    name: string
): AppThunk => (dispatch, getState) => {
    const text = selector(getState());
    if (text) {
        navigator.clipboard.writeText(text);
        dispatch(notification({ title: `${name} copied to clipboard`, text }));
    }
};

const handlers: { key: string; action: AppThunk }[] = [
    {
        key: 'c',
        action: copyAction((state) => {
            const currentWord = selectSelectedWord(state);
            return selectSelectedDialog(state)
                ?.words.map((line) =>
                    line
                        .map((word) =>
                            word.baseForm === currentWord.wordBaseForm ||
                            isKana(word.displayText) ||
                            word.displayText === word.reading ||
                            !word.reading
                                ? word.displayText
                                : ` ${word.displayText}[${word.reading}]`
                        )
                        .join('')
                )
                .join('\n');
        }, 'Japanese text')
    },
    {
        key: 'd',
        action: copyAction(
            (state) => selectSelectedEnglishDialog(state)?.text?.join('\n'),
            'English text'
        )
    },
    {
        key: 'x',
        action: copyAction((state) => {
            const word = selectSelectedWord(state).wordBaseForm;
            if (!word) {
                return;
            }

            return selectDefinitionById(state, word)?.exact[0].japanese[0]
                .reading;
        }, 'word reading')
    },
    {
        key: 't',
        action: (dispatch, getState) => {
            const word = selectSelectedWord(getState()).wordBaseForm;
            if (word) {
                dispatch(toggleWordFurigana(word));
            }
        }
    }
];

export const handler = (e: KeyboardEvent) => {
    const action = handlers.find((a) => a.key === e.key)?.action;
    if (action) {
        store.dispatch(action);
    }
};

declare global {
    interface Window {
        oldHandler?: typeof handler;
    }
}

if (window.oldHandler) {
    console.log('removing old keypress handler');
    window.removeEventListener('keypress', window.oldHandler);
}

window.addEventListener('keypress', handler);
window.oldHandler = handler;
