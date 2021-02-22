import { mdiImport } from '@mdi/js';
import Icon from '@mdi/react';
import { useTypedSelector } from 'app/hooks';
import { Dialog } from 'features/dialog/Dialog';
import React, { FC, useState } from 'react';
import { useDispatch } from 'react-redux';
import styles from './wordContext.module.scss';
import { useWordsOccurrencesQuery } from 'backend';
import { selectAnalyzer } from '../backendSelection';
import { QueryPlaceholder } from '../../components/placeholder/queryPlaceholder';

const max = 50;
const ICON_SIZE = '2em';
export const WordOccurrences: FC<{ wordId: number; readOnly?: boolean }> = ({
    wordId,
    readOnly
}) => {
    const [skip, setSkip] = useState(0);
    const dispatch = useDispatch();
    const analyzer = useTypedSelector(selectAnalyzer);
    const occurrences = useWordsOccurrencesQuery({
        analyzer,
        wordId
    });

    if (!occurrences.data) {
        return <QueryPlaceholder result={occurrences} />;
    }

    const dialogIds = occurrences.data.dialogIds.slice(skip, skip + max);
    return (
        <>
            {dialogIds.map((con) => (
                <div className={styles.dialog} key={con}>
                    <Dialog dialogId={con} showTitle forWord={wordId} />
                </div>
            ))}
        </>
    );
};
