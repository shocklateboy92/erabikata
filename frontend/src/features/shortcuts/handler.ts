import { RootState } from 'app/rootReducer';
import { AppThunk } from 'app/store';
import { notification } from 'features/notifications';
import { selectSelectedDialog } from 'features/selectedWord';

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
        action: copyAction(
            (state) =>
                selectSelectedDialog(state)
                    ?.words.map((line) =>
                        line
                            .map((word) =>
                                isKana(word.displayText) ||
                                word.displayText === word.reading
                                    ? word.displayText
                                    : `${word.displayText}[${word.reading}]`
                            )
                            .join('')
                    )
                    .join('\n'),
            'Japanese text'
        )
    }
];

export const getMatchingAction = (e: React.KeyboardEvent) => {
    const action = handlers.find((a) => a.key === e.key)?.action;
    return action;
};
