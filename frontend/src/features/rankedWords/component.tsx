import { mdiShare } from '@mdi/js';
import Icon from '@mdi/react';
import { useTypedSelector } from 'app/hooks';
import { RootState } from 'app/rootReducer';
import classNames from 'classnames';
import {
    newWordSelected,
    selectSelectedWord,
    WordContext
} from 'features/selectedWord';
import React, { FC, useEffect } from 'react';
import { shallowEqual, useDispatch, useSelector } from 'react-redux';
import { Link, useParams } from 'react-router-dom';
import styles from './rankedWords.module.scss';
import { fetchRankedWords, selectRankedWordsArray } from './slice';

const SelectableDiv: FC<{ word: string }> = ({ word }) => {
    const isActive = useTypedSelector(
        (state) => selectSelectedWord(state).wordBaseForm === word
    );
    const dispatch = useDispatch();
    return (
        <div
            className={classNames(styles.word, {
                [styles.active]: isActive
            })}
            onClick={() => dispatch(newWordSelected({ word }))}
        >
            <ruby>{word}</ruby>
            <WordContext word={word} />
            <Link to={`/word/${word}`}>
                <Icon path={mdiShare} size="2em" />
            </Link>
        </div>
    );
};

export const RankedWords: FC = () => {
    const pageParam = parseInt(useParams<{ pageNum: string }>().pageNum);
    const pageNum = isNaN(pageParam) ? 0 : pageParam;

    const words = useSelector(
        (state: RootState) => selectRankedWordsArray(state, pageNum),
        shallowEqual
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
                    <SelectableDiv key={word.rank} word={word.word} />
                ))}
            </div>
            <Link to={`/rankedWords/${pageNum + 1}`}>Next Page</Link>
        </div>
    );
};
