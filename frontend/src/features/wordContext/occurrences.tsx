import { mdiImport } from '@mdi/js';
import Icon from '@mdi/react';
import { useTypedSelector } from 'app/hooks';
import { Dialog } from 'features/dialog/Dialog';
import { dialogSelection } from 'features/selectedWord';
import React, { FC, useState } from 'react';
import { useDispatch } from 'react-redux';
import styles from './wordContext.module.scss';
import { useSubsByIdStringQuery, useWordsOccurrencesQuery } from 'backend';
import { selectAnalyzer } from '../backendSelection';
import { QueryPlaceholder } from "../../components/placeholder/queryPlaceholder";

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
        wordId,
        skip,
        max
    });


    if (!occurrences.data) {
        return <QueryPlaceholder result={occurrences} />;
    }

    return (
        <>
            {occurrences.data.dialogIds.map((con) => (
                <div className={styles.dialog} key={con}>
                    <Dialog
                        dialogId={con}
                        showTitle
                        readOnly={readOnly}
                    >
                        {readOnly && (
                            <span
                                role="button"
                                onClick={() => {
                                    // dispatch(
                                        // dialogSelection({
                                        //     time: con.time,
                                        //     episode: con.episodeId
                                        // })
                                    // );
                                }}
                            >
                                <Icon path={mdiImport} size={ICON_SIZE} />
                            </span>
                        )}
                    </Dialog>
                </div>
            ))}
        </>
    );
};
