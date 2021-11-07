import { useTypedSelector } from 'app/hooks';
import { useWordsOccurrencesQuery } from 'backend';
import { Dialog } from 'features/dialog/Dialog';
import React, { FC } from 'react';
import { QueryPlaceholder } from '../../components/placeholder/queryPlaceholder';
import { selectAnalyzer } from '../backendSelection';
import styles from './wordContext.module.scss';

const max = 50;
export const WordOccurrences: FC<{ wordId: number; readOnly?: boolean }> = ({
    wordId,
    readOnly
}) => {
    const occurrences = useWordsOccurrencesQuery({
        wordId
    });

    if (!occurrences.data) {
        return <QueryPlaceholder result={occurrences} />;
    }

    const dialogIds = occurrences.data.dialogIds.slice(0, max);
    return (
        <>
            {dialogIds.map((con) => (
                <div className={styles.dialog} key={con}>
                    <Dialog
                        dialogId={con}
                        showTitle
                        forWord={readOnly ? wordId : undefined}
                    />
                </div>
            ))}
        </>
    );
};
