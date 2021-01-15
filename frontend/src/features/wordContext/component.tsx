import { useTypedSelector } from 'app/hooks';
import { fetchWordIfNeeded, selectWordInfo } from 'features/wordContext';
import React, { FC, useEffect } from 'react';
import { useDispatch } from 'react-redux';
import styles from './wordContext.module.scss';

export const WordContext: FC<{ word: string }> = ({ word }) => {
    const dispatch = useDispatch();
    useEffect(() => {
        dispatch(fetchWordIfNeeded({ baseForm: word }));
    });

    const wordContext = useTypedSelector((state) =>
        selectWordInfo(word, state)
    );

    return (
        <div className={styles.text}>
            Rank: {wordContext?.rank ?? '????'}
            <br />
            Occurrences: {wordContext?.totalOccurrences ?? '????'}
        </div>
    );
};