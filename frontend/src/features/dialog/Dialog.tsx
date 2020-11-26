import { useAppSelector } from 'app/hooks';
import classNames from 'classnames';
import { InlineError } from 'components/inlineError';
import { Ruby } from 'components/ruby';
import { selectIsFuriganaEnabled } from 'features/furigana';
import { newWordSelected, selectSelectedWord } from 'features/selectedWord';
import { fetchWordIfNeeded } from 'features/wordContext';
import moment from 'moment';
import React, { FC } from 'react';
import { useDispatch } from 'react-redux';
import styles from './dialog.module.scss';
import { selectDialogContent } from './slice';

export const Dialog: FC<{
    episode: string;
    time: number;
    title?: string;
    readOnly?: boolean;
}> = ({ time, episode, title, readOnly, children }) => {
    const dispatch = useDispatch();
    const furiganaEnabled = useAppSelector(selectIsFuriganaEnabled);
    const dialog = useAppSelector(
        selectDialogContent.bind(null, episode, time)
    );
    const selectedWord = useAppSelector(selectSelectedWord);

    if (!dialog) {
        return (
            <InlineError>
                Trying to display a dialog that has not been fetched.
            </InlineError>
        );
    }

    return (
        <div className={styles.container}>
            <div className={styles.metadata}>
                {moment.utc(dialog.startTime * 1000).format('H:mm:ss')} {title}
            </div>
            <div>
                {dialog.words.map((word, index) => (
                    <Ruby
                        key={index}
                        className={classNames({
                            [styles.active]:
                                selectedWord?.episode === episode &&
                                selectedWord.sentenceTimestamp ===
                                    dialog.startTime &&
                                selectedWord.wordBaseForm === word.baseForm
                        })}
                        onClick={() => {
                            if (readOnly) {
                                return;
                            }

                            dispatch(
                                newWordSelected({
                                    word: word.baseForm,
                                    timestamp: dialog.startTime,
                                    episode
                                })
                            );
                            dispatch(
                                fetchWordIfNeeded({ baseForm: word.baseForm })
                            );
                        }}
                        reading={word.reading}
                        hideReading={!furiganaEnabled}
                    >
                        {word.displayText}
                    </Ruby>
                ))}
                {children}
            </div>
        </div>
    );
};
