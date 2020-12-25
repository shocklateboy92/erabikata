import { useTypedSelector } from 'app/hooks';
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
    const furiganaEnabled = useTypedSelector(selectIsFuriganaEnabled);
    const dialog = useTypedSelector(
        selectDialogContent.bind(null, episode, time)
    );

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
                {dialog.words.map((line) => (
                    <div>
                        {line.map((word, index) => (
                            <SelectableRuby
                                key={index}
                                episode={episode}
                                time={dialog.startTime}
                                word={word.baseForm}
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
                                        fetchWordIfNeeded({
                                            baseForm: word.baseForm
                                        })
                                    );
                                }}
                                reading={word.reading}
                                hideReading={!furiganaEnabled}
                            >
                                {word.displayText}
                            </SelectableRuby>
                        ))}
                    </div>
                ))}
                {children}
            </div>
        </div>
    );
};

const SelectableRuby: FC<
    { episode: string; time: number; word: string } & React.ComponentProps<
        typeof Ruby
    >
> = ({ episode, time, word, ...restProps }) => {
    const isActive = useTypedSelector((state) => {
        const selectedWord = selectSelectedWord(state);
        return (
            selectedWord?.episode === episode &&
            selectedWord.sentenceTimestamp === time &&
            selectedWord.wordBaseForm === word
        );
    });

    return (
        <Ruby
            className={classNames({ [styles.active]: isActive })}
            {...restProps}
        />
    );
};
