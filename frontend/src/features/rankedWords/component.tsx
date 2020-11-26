import { useAppSelector } from 'app/hooks';
import { RootState } from 'app/rootReducer';
import { selectWordInfo } from 'features/wordContext';
import React, { FC, useEffect } from 'react';
import { useDispatch } from 'react-redux';
import { fetchRankedWords, selectRankedWords } from './slice';
import styles from './rankedWords.module.scss';
import Icon from '@mdi/react';
import { mdiShare } from '@mdi/js';
import { Link } from 'react-router-dom';
import { newWordSelected, selectSelectedWord } from 'features/selectedWord';
import classNames from 'classnames';

export const RankedWords: FC = () => {
    const words = useAppSelector((state: RootState) =>
        selectRankedWords(state).map((word) => selectWordInfo(word.text, state))
    );
    const selectedWord = useAppSelector(selectSelectedWord);

    const dispatch = useDispatch();
    useEffect(() => {
        dispatch(fetchRankedWords({}));
    }, [dispatch]);

    return (
        <div className={styles.container}>
            {words.map((word) => (
                <div
                    key={word?.text}
                    className={classNames(styles.word, {
                        [styles.active]:
                            selectedWord?.wordBaseForm === word?.text
                    })}
                >
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
                </div>
            ))}
        </div>
    );
};
