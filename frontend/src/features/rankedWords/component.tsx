import { mdiShare } from '@mdi/js';
import Icon from '@mdi/react';
import { useTypedSelector } from 'app/hooks';
import classNames from 'classnames';
import { newWordSelected, selectSelectedWord } from 'features/selectedWord';
import React, { FC, useEffect } from 'react';
import { useDispatch } from 'react-redux';
import { Link, useParams } from 'react-router-dom';
import styles from './rankedWords.module.scss';
import { fetchRankedWords, selectRankedWords } from './slice';

const SelectableDiv: FC<{ word: string }> = ({ word, children }) => {
    const selectedWord = useTypedSelector(selectSelectedWord);
    return (
        <div
            className={classNames(styles.word, {
                [styles.active]: selectedWord?.wordBaseForm === word
            })}
        >
            {children}
        </div>
    );
};

export const RankedWords: FC = () => {
    const pageParam = parseInt(useParams<{ pageNum: string }>().pageNum);
    const pageNum = isNaN(pageParam) ? 0 : pageParam;

    const words = useTypedSelector((state) =>
        selectRankedWords(state, pageNum)
    );

    const dispatch = useDispatch();
    useEffect(() => {
        dispatch(fetchRankedWords({ pageNum }));
    }, [dispatch, pageNum]);

    return (
        <div className={styles.outer}>
            {pageNum > 0 && (
                <Link to={`/rankedWords/${pageNum - 1}`}>Previous Page</Link>
            )}
            <div className={styles.container}>
                {words.map((word) => (
                    <SelectableDiv key={word.rank} word={word.text}>
                        <ruby>{word?.text}</ruby>
                        <div
                            className={styles.info}
                            onClick={() =>
                                dispatch(newWordSelected({ word: word?.text! }))
                            }
                        >
                            <div>Rank: {word?.rank}</div>
                            <div>Occurrences: {word?.totalOccurrences}</div>
                        </div>
                        <Link to={`/word/${word?.text}`}>
                            <Icon path={mdiShare} size="2em" />
                        </Link>
                    </SelectableDiv>
                ))}
            </div>
            <Link to={`/rankedWords/${pageNum + 1}`}>Next Page</Link>
        </div>
    );
};
