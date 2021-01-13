import { RootState } from 'app/rootReducer';
import store, { AppThunk } from 'app/store';
import { notification } from 'features/notifications';
import {
    selectSelectedDialog,
    selectSelectedEnglishDialog,
    selectSelectedWord
} from 'features/selectedWord';

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

const isKana = (text: string) =>
    text.match(/^[\u3000-\u303f\u3040-\u309f\u30a0-\u30ff\uff00-\uff9f]*$/);

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
                                : `${word.displayText}[${word.reading}]`
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
