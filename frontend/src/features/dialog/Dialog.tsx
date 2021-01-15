import { useTypedSelector } from 'app/hooks';
import classNames from 'classnames';
import { InlineError } from 'components/inlineError';
import { Ruby } from 'components/ruby';
import { formatTime } from 'components/time';
import { selectIsFuriganaEnabled } from 'features/furigana';
import { newWordSelected, selectSelectedWord } from 'features/selectedWord';
import React, { FC, Fragment } from 'react';
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
                {formatTime(dialog.startTime)} {title}
            </div>
            <div className={styles.lines}>
                {dialog.words.map((line, lineIndex) => (
                    <Fragment key={lineIndex}>
                        {lineIndex > 0 && <br />}
                        {line.map((word, index) => (
                            <SelectableRuby
                                key={index}
                                episode={episode}
                                alwaysHighlightSelectedWord={readOnly}
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
                                }}
                                reading={word.reading}
                                hideReading={!furiganaEnabled}
                            >
                                {
                                    // It seems the analyzers replace japanese spaces with normal spaces
                                    word.displayText.replaceAll(' ', '　')
                                }
                            </SelectableRuby>
                        ))}
                    </Fragment>
                ))}
                {children}
            </div>
        </div>
    );
};

const SelectableRuby: FC<
    {
        episode: string;
        time: number;
        word: string;
        alwaysHighlightSelectedWord?: boolean;
    } & React.ComponentProps<typeof Ruby>
> = ({ episode, time, word, alwaysHighlightSelectedWord, ...restProps }) => {
    const isActive = useTypedSelector((state) => {
        const selectedWord = selectSelectedWord(state);
        return (
            ((selectedWord?.episode === episode &&
                selectedWord.sentenceTimestamp === time) ||
                alwaysHighlightSelectedWord) &&
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
