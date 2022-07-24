import { RootState } from 'app/rootReducer';
import store, { AppThunk } from 'app/store';
import history from 'appHistory';
import { apiEndpoints } from 'backend';
import { selectSentenceTextToSend } from 'features/anki/selectors';
import { toggleWordFurigana } from 'features/furigana';
import { playFrom, togglePlayback } from 'features/hass';
import { notification } from 'features/notifications';
import {
    dialogWordShift,
    episodeDialogShift,
    occurrenceShift,
    selectedWordCycleRequest,
    selectedWordReverseCycleRequest,
    selectNearestSelectedDialog,
    selectSelectedEpisodeContent,
    selectSelectedWord,
    selectSelectedWordOccurrences,
    selectSelectedWords,
    wordSelectionV2
} from 'features/selectedWord';
import {
    selectDefinitionsById,
    selectSelectedWordDefinitions
} from 'features/wordDefinition';
import { generateDialogLink } from 'routing/linkGen';
import { selectSelectedEnglishDialog } from '../engDialog';

const copyAction =
    (
        selector: (state: RootState) => string | undefined | null,
        name: string
    ): AppThunk =>
    (dispatch, getState) => {
        const text = selector(getState());
        if (text) {
            navigator.clipboard.writeText(text);
            dispatch(
                notification({ title: `${name} copied to clipboard`, text })
            );
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
        action: copyAction(
            (state) => selectSentenceTextToSend(state),
            'Japanese text'
        )
    },
    {
        key: 'C',
        action: (_, getState) => {
            const { wordIds, episode, sentenceTimestamp } = selectSelectedWord(
                getState()
            );
            if (!episode || sentenceTimestamp === undefined) {
                return;
            }

            history.push(
                generateDialogLink(episode, sentenceTimestamp, wordIds)
            );
        }
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
            dispatch(toggleWordFurigana(selectSelectedWords(getState())[0]));
        }
    },
    {
        key: 'l',
        action: (dispatch, getState) =>
            dispatch(
                dialogWordShift({
                    direction: 1,
                    dialog: selectNearestSelectedDialog(getState())
                })
            )
    },
    {
        key: 'h',
        action: (dispatch, getState) =>
            dispatch(
                dialogWordShift({
                    direction: -1,
                    dialog: selectNearestSelectedDialog(getState())
                })
            )
    },
    {
        key: 'j',
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
        key: 'k',
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
        key: 'h',
        alt: true,
        action: (dispatch) => {
            dispatch(selectedWordCycleRequest());
        }
    },
    {
        key: 'l',
        alt: true,
        action: (dispatch) => {
            dispatch(selectedWordReverseCycleRequest());
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
    },

    {
        key: ' ',
        action: (dispatch) => dispatch(togglePlayback())
    },
    {
        key: 'i',
        action: (dispatch) => dispatch(togglePlayback())
    },
    {
        key: 'p',
        action: (dispatch, getState) => {
            const timeStamp = getState().selectedWord.sentenceTimestamp;
            if (timeStamp) {
                dispatch(playFrom({ timeStamp }));
            }
        }
    },

    {
        key: 'o',
        action: (dispatch, getState) => {
            const state = getState();
            const dialog = selectNearestSelectedDialog(state);
            const { data: known } = apiEndpoints.wordsKnown.select()(state);
            const firstUnkown = dialog?.words
                .flat()
                .find((word) =>
                    word.definitionIds.find((id) => !known?.[id.toString()])
                );

            if (firstUnkown) {
                dispatch(wordSelectionV2(firstUnkown.definitionIds));
            }
        }
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
