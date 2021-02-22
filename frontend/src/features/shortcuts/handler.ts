import { RootState } from 'app/rootReducer';
import store, { AppThunk } from 'app/store';
import {
    isKana,
    selectIsFuriganaHiddenForWord,
    toggleWordFurigana
} from 'features/furigana';
import { notification } from 'features/notifications';
import {
    dialogWordShift,
    episodeDialogShift,
    occurrenceShift,
    selectNearestSelectedDialog,
    selectSelectedEnglishDialog,
    selectSelectedEpisodeContent,
    selectSelectedWord,
    selectSelectedWordOccurrences,
    selectSelectedWords
} from 'features/selectedWord';
import {
    selectDefinitionsById,
    selectSelectedWordDefinitions
} from 'features/wordDefinition';

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

const handlers: {
    key: string;
    alt?: boolean;
    ctrl?: boolean;
    action: AppThunk;
}[] = [
    {
        key: 'c',
        action: copyAction((state) => {
            const selectedWords = selectSelectedWords(state);
            const selectedDialog = selectNearestSelectedDialog(state);
            return selectedDialog?.words
                .map((line) =>
                    line
                        .map((word) =>
                            word.definitionIds === selectedWords ||
                            isKana(word.displayText) ||
                            selectIsFuriganaHiddenForWord(
                                state,
                                word.baseForm
                            ) ||
                            word.displayText === word.reading ||
                            !word.reading
                                ? word.displayText.replaceAll(' ', '　')
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
            const word = selectSelectedWord(state).wordIds;
            if (!word) {
                return;
            }

            return selectDefinitionsById(state, word)[0]?.japanese[0].reading;
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
    },
    {
        key: 'L',
        action: (dispatch, getState) =>
            dispatch(
                dialogWordShift({
                    direction: 1,
                    dialog: selectNearestSelectedDialog(getState())
                })
            )
    },
    {
        key: 'H',
        action: (dispatch, getState) =>
            dispatch(
                dialogWordShift({
                    direction: -1,
                    dialog: selectNearestSelectedDialog(getState())
                })
            )
    },
    {
        key: 'J',
        action: (dispatch, getState) => {
            dispatch(
                episodeDialogShift({
                    direction: 1,
                    episodeDialog: selectSelectedEpisodeContent(getState())
                })
            );
        }
    },
    {
        key: 'K',
        action: (dispatch, getState) => {
            dispatch(
                episodeDialogShift({
                    direction: -1,
                    episodeDialog: selectSelectedEpisodeContent(getState())
                })
            );
        }
    },
    {
        key: 'j',
        alt: true,
        action: (dispatch, getState) => {
            dispatch(
                occurrenceShift({
                    direction: 1,
                    context: selectSelectedWordOccurrences(getState())
                })
            );
        }
    },
    {
        key: 'k',
        alt: true,
        action: (dispatch, getState) => {
            dispatch(
                occurrenceShift({
                    direction: -1,
                    context: selectSelectedWordOccurrences(getState())
                })
            );
        }
    },
    {
        key: 's',
        action: copyAction((state) => {
            const definiton = selectSelectedWordDefinitions(state)[0];
            if (!definiton) {
                return;
            }

            return definiton.english
                .map((meaning) => meaning.senses.join('\n'))
                .join('\n\n');
        }, 'Primary word definition')
    },
    {
        key: 'a',
        action: copyAction((state) => {
            const definiton = selectSelectedWordDefinitions(state)[0];
            if (!definiton) {
                return;
            }

            return definiton.english
                .map((word) => word.tags.join(', '))
                .join('\n\n');
        }, 'Primary word notes')
    }
];

export const handler = (e: KeyboardEvent) => {
    const action = handlers.find(
        (a) => a.key === e.key && e.altKey === !!a.alt && e.ctrlKey === !!a.ctrl
    )?.action;
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
    window.removeEventListener('keydown', window.oldHandler);
}

window.addEventListener('keydown', handler);
window.oldHandler = handler;
