import { mdiImport, mdiRadioboxMarked, mdiShare } from '@mdi/js';
import Icon from '@mdi/react';
import { useTypedSelector } from 'app/hooks';
import { useSubsByIdQuery, useWordsKnownQuery } from 'backend';
import classNames from 'classnames';
import { Row } from 'components/layout';
import { formatTime } from 'components/time';
import { Ruby } from 'features/furigana';
import {
    dialogSelection,
    dialogWordSelectionV2,
    encodeSelectionParams,
    selectIsCurrentlySelected,
    selectSelectedWord
} from 'features/selectedWord';
import React, { FC, Fragment, useEffect, useRef } from 'react';
import { useDispatch } from 'react-redux';
import { Link } from 'react-router-dom';
import { QueryPlaceholder } from '../../components/placeholder/queryPlaceholder';
import { selectAnalyzer } from '../backendSelection';
import './dialog.scss';

const ICON_SIZE = '2em';
export const Dialog: FC<{
    dialogId: string;
    showTitle?: boolean;
    compact?: boolean;
    autoSelect?: boolean;
    scrollTo?: boolean;
    forWord?: number;
}> = ({ forWord, dialogId, compact, showTitle, autoSelect, scrollTo }) => {
    const dispatch = useDispatch();
    const analyzer = useTypedSelector(selectAnalyzer);
    const ref = useRef<HTMLDivElement>(null);
    const { data, ...result } = useSubsByIdQuery({ id: dialogId, analyzer });
    const isActive = useTypedSelector((state) =>
        selectIsCurrentlySelected(state, data?.episodeId, data?.time)
    );
    const knownWords = useWordsKnownQuery(
        {},
        {
            selectFromResult: (result) =>
                data
                    ? Object.fromEntries(
                          data.text.words.flatMap((l) =>
                              l.flatMap((w) =>
                                  w.definitionIds.map((d) => [
                                      d,
                                      !!result.data?.[d.toString()]
                                  ])
                              )
                          )
                      )
                    : {}
        }
    );

    useEffect(() => {
        if (!data) {
            return;
        }

        if (autoSelect) {
            dispatch(
                dialogSelection({
                    time: data.time,
                    episode: data.episodeId
                })
            );
        }

        if (scrollTo) {
            ref.current?.scrollIntoView({ block: 'nearest' });
        }
    }, [dispatch, autoSelect, scrollTo, data]);

    if (!data) {
        return <QueryPlaceholder result={result} quiet />;
    }
    const { text, episodeName, episodeId, time } = data;

    return (
        <div className="dialog-container" ref={ref}>
            {!compact && (
                <Row>
                    {isActive && <Icon path={mdiRadioboxMarked} />}
                    <span className="content">
                        {formatTime(text.startTime)} {showTitle && episodeName}
                    </span>
                </Row>
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
                                knownWords={knownWords}
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
        knownWords: { [key: number]: boolean };
        alwaysHighlightSelectedWord?: boolean;
    } & React.ComponentProps<typeof Ruby>
> = ({
    episode,
    time,
    wordIds,
    knownWords,
    alwaysHighlightSelectedWord,
    ...restProps
}) => {
    const active = useTypedSelector((state) => {
        const selectedWord = selectSelectedWord(state);
        return (
            (alwaysHighlightSelectedWord ||
                (selectedWord.sentenceTimestamp === time &&
                    selectedWord?.episode === episode)) &&
            selectedWord.wordIds.find((a) => wordIds.includes(a))
        );
    });
    const known = wordIds.every((w) => knownWords[w]);

    return (
        <Ruby
            className={classNames({ active, known })}
            wordIds={wordIds}
            role="button"
            {...restProps}
        />
    );
};
