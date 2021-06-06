import { useTypedSelector } from 'app/hooks';
import { Dialog } from 'features/dialog/Dialog';
import React, { FC, useState } from 'react';
import styles from './wordContext.module.scss';
import { useWordsOccurrencesQuery } from 'backend';
import { selectAnalyzer } from '../backendSelection';
import { QueryPlaceholder } from '../../components/placeholder/queryPlaceholder';

const max = 50;
export const WordOccurrences: FC<{ wordId: number; readOnly?: boolean }> = ({
    wordId,
    readOnly
}) => {
    const analyzer = useTypedSelector(selectAnalyzer);
    const occurrences = useWordsOccurrencesQuery({
        analyzer,
        wordId
    });

    if (!occurrences.data) {
        return <QueryPlaceholder result={occurrences} />;
    }

    return (
        <>
            {Object.entries(occurrences.data.dialogIds).map(([pos, ids]) => (
                <div key={pos}>
                    <h4>{pos}</h4>
                    <WordOccurrencesGroup
                        dialogIds={ids}
                        forWord={readOnly ? wordId : undefined}
                    />
                </div>
            ))}
        </>
    );
};

const WordOccurrencesGroup: FC<{ dialogIds: string[]; forWord?: number }> = ({
    dialogIds,
    forWord
}) => {
    const [skip] = useState(0);
    const pagedIds = dialogIds.slice(skip, skip + max);
    return (
        <>
            {pagedIds.map((con) => (
                <div className={styles.dialog} key={con}>
                    <Dialog dialogId={con} showTitle forWord={forWord} />
                </div>
            ))}
        </>
    );
};
