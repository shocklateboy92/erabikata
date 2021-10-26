import { useTypedSelector } from 'app/hooks';
import classNames from 'classnames';
import { formatTime } from 'components/time';
import { Ruby } from 'features/furigana';
import {
    dialogSelection,
    dialogWordSelectionV2,
    encodeSelectionParams,
    selectIsCurrentlySelected,
    selectSelectedEpisodeTime,
    selectSelectedWord
} from 'features/selectedWord';
import React, { FC, Fragment, useEffect } from 'react';
import { useDispatch } from 'react-redux';
import './dialog.scss';
import { useSubsByIdQuery, useWordsKnownQuery } from 'backend';
import { selectAnalyzer } from '../backendSelection';
import { QueryPlaceholder } from '../../components/placeholder/queryPlaceholder';
import Icon from '@mdi/react';
import { mdiImport, mdiRadioboxMarked, mdiShare } from '@mdi/js';
import { Link } from 'react-router-dom';
import { Row } from 'components/layout';

const ICON_SIZE = '2em';
export const Dialog: FC<{
    dialogId: string;
    showTitle?: boolean;
    compact?: boolean;
    autoSelect?: boolean;
    forWord?: number;
}> = ({ forWord, dialogId, compact, showTitle, autoSelect }) => {
    const dispatch = useDispatch();
    const analyzer = useTypedSelector(selectAnalyzer);
    const result = useSubsByIdQuery({ id: dialogId, analyzer });
    const data = result.data;
    const isActive = useTypedSelector((state) =>
        selectIsCurrentlySelected(state, data?.episodeId, data?.time)
    );
    useEffect(() => {
        if (!result.data || !autoSelect) {
            return;
        }

        dispatch(
            dialogSelection({
                time: result.data.time,
                episode: result.data.episodeId
            })
        );
    }, [dispatch, autoSelect, result.data]);

    if (!result.data) {
        return <QueryPlaceholder result={result} quiet />;
    }
    const { text, episodeName, episodeId, time } = result.data;

    return (
        <div className="dialog-container">
            {!compact && (
                <div className="metadata">
                    <Row>
                        {isActive && <Icon path={mdiRadioboxMarked} />}
                        <span>
                            {formatTime(text.startTime)}{' '}
                            {showTitle && episodeName}
                        </span>
                    </Row>
                </div>
            )}
            <div className="lines">
                {text.words.map((line, lineIndex) => (
                    <Fragment key={lineIndex}>
                        {lineIndex > 0 && <br />}
                        {line.map((word, index) => (
                            <SelectableRuby
                                key={index}
                                episode={episodeId}
                                alwaysHighlightSelectedWord={!!forWord}
                                time={text.startTime}
                                wordIds={word.definitionIds}
                                onClick={() => {
                                    if (forWord) {
                                        return;
                                    }

                                    dispatch(
                                        dialogWordSelectionV2({
                                            baseForm: word.baseForm,
                                            time: text.startTime,
                                            episodeId,
                                            wordIds: word.definitionIds
                                        })
                                    );
                                }}
                                reading={word.reading}
                            >
                                {
                                    // It seems the analyzers replace japanese spaces with normal spaces
                                    word.displayText
                                }
                            </SelectableRuby>
                        ))}
                    </Fragment>
                ))}
                {forWord && (
                    <>
                        <Link
                            to={{
                                pathname: '/ui/dialog',
                                search: encodeSelectionParams(episodeId, time, [
                                    forWord
                                ])
                            }}
                        >
                            <Icon path={mdiShare} size={ICON_SIZE} />
                        </Link>
                        <span
                            role="button"
                            onClick={() => {
                                dispatch(
                                    dialogSelection({
                                        time,
                                        episode: episodeId
                                    })
                                );
                            }}
                        >
                            <Icon path={mdiImport} size={ICON_SIZE} />
                        </span>
                    </>
                )}
            </div>
        </div>
    );
};

const SelectableRuby: FC<
    {
        episode: string;
        time: number;
        wordIds: number[];
        alwaysHighlightSelectedWord?: boolean;
    } & React.ComponentProps<typeof Ruby>
> = ({ episode, time, wordIds, alwaysHighlightSelectedWord, ...restProps }) => {
    const active = useTypedSelector((state) => {
        const selectedWord = selectSelectedWord(state);
        return (
            ((selectedWord?.episode === episode &&
                selectedWord.sentenceTimestamp === time) ||
                alwaysHighlightSelectedWord) &&
            selectedWord.wordIds.find((a) => wordIds.includes(a))
        );
    });
    const { known } = useWordsKnownQuery(
        {},
        {
            selectFromResult: (result) => ({
                known: wordIds.every((id) => result.data?.[id])
            })
        }
    );

    return (
        <Ruby
            className={classNames({ active, known })}
            wordIds={wordIds}
            {...restProps}
        />
    );
};
