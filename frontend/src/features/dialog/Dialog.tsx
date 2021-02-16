import { useTypedSelector } from 'app/hooks';
import classNames from 'classnames';
import { InlineError } from 'components/inlineError';
import { formatTime } from 'components/time';
import { Ruby } from 'features/furigana';
import {
    dialogWordSelectionV2,
    selectSelectedWord
} from 'features/selectedWord';
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
                                baseForm={word.baseForm}
                                onClick={() => {
                                    if (readOnly) {
                                        return;
                                    }

                                    dispatch(
                                        dialogWordSelectionV2({
                                            baseForm: word.baseForm,
                                            time: dialog.startTime,
                                            episodeId: episode,
                                            wordIds: word.definitionIds
                                        })
                                    );
                                }}
                                reading={word.reading}
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
        baseForm: string;
        alwaysHighlightSelectedWord?: boolean;
    } & React.ComponentProps<typeof Ruby>
> = ({
    episode,
    time,
    baseForm: word,
    alwaysHighlightSelectedWord,
    ...restProps
}) => {
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
            baseForm={word}
            {...restProps}
        />
    );
};
