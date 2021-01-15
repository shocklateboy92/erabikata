import { mdiImport } from '@mdi/js';
import Icon from '@mdi/react';
import { useTypedSelector } from 'app/hooks';
import { Dialog } from 'features/dialog/Dialog';
import { dialogSelection } from 'features/selectedWord';
import { WordLink } from 'features/selectedWord/wordLink';
import React, { FC, useEffect } from 'react';
import { useDispatch } from 'react-redux';
import { fetchWordIfNeeded } from './api';
import { selectWordInfo } from './slice';
import styles from './wordContext.module.scss';

const ICON_SIZE = '2em';
export const WordOccurrences: FC<{ word: string }> = (props) => {
    const context = useTypedSelector((state) =>
        selectWordInfo(props.word, state)
    );
    const dispatch = useDispatch();
    useEffect(() => {
        dispatch(
            fetchWordIfNeeded({ baseForm: props.word, pagingInfo: { max: 40 } })
        );
    });

    return (
        <>
            {context?.occurrences.map((con) => (
                <div className={styles.dialog} key={con.episodeName + con.time}>
                    <Dialog
                        readOnly
                        episode={con.episodeId}
                        time={con.time}
                        title={con.episodeName}
                        dialogFreeSelection
                    >
                        <WordLink
                            word={props.word}
                            includeEpisode={con.episodeId}
                            includeTime={con.time}
                            iconSize={ICON_SIZE}
                        />
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
