import { mdiImport } from '@mdi/js';
import Icon from '@mdi/react';
import { useTypedSelector } from 'app/hooks';
import { Dialog } from 'features/dialog/Dialog';
import { dialogSelection } from 'features/selectedWord';
import { WordLink } from 'features/selectedWord/wordLink';
import React, { FC, useEffect, useState } from 'react';
import { useDispatch } from 'react-redux';
import { fetchWordIfNeeded } from './api';
import { selectWordInfo } from './slice';
import styles from './wordContext.module.scss';
import { useSubsByIdStringQuery, useWordsOccurrencesQuery } from 'backend';
import { selectAnalyzer } from '../backendSelection';

const max = 50;
const ICON_SIZE = '2em';
export const WordOccurrences: FC<{ wordId: number }> = ({ wordId }) => {
    const [skip, setSkip] = useState(0);
    const dispatch = useDispatch();
    const analyzer = useTypedSelector(selectAnalyzer);
    const occurrences = useWordsOccurrencesQuery({
        analyzer,
        wordId,
        skip,
        max
    });

    const dialog = useSubsByIdStringQuery(
        {
            analyzer,
            dialogIds: occurrences.data?.dialogIds.join(',') ?? ''
        },
        { skip: !occurrences.data?.dialogIds.length }
    );

    if (occurrences.isLoading || dialog.isLoading) {
        return <>Now Loading...</>;
    }

    if (!dialog.data) {
        return (
            <pre>
                {JSON.stringify(occurrences.error ?? dialog.error, null, 2)}
            </pre>
        );
    }

    return (
        <>
            {dialog.data.map((con) => (
                <div className={styles.dialog} key={con.episodeName + con.time}>
                    <Dialog
                        readOnly
                        content={con}
                    >
                        <span
                            role="button"
                            onClick={() => {
                                dispatch(
                                    dialogSelection({
                                        time: con.time,
                                        episode: con.episodeId
                                    })
                                );
                            }}
                        >
                            <Icon path={mdiImport} size={ICON_SIZE} />
                        </span>
                    </Dialog>
                </div>
            ))}
        </>
    );
};
