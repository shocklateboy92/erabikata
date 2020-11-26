import { useAppSelector } from 'app/hooks';
import { Drawer } from 'components/drawer';
import { Separator } from 'components/separator';
import { Dialog } from 'features/dialog/Dialog';
import { selectSelectedWord } from 'features/selectedWord';
import { WordLink } from 'features/selectedWord/wordLink';
import { fetchWordIfNeeded, selectWordInfo } from 'features/wordContext';
import { WordDefinition } from 'features/wordDefinition';
import React, { FC, useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import styles from './wordSummary.module.scss';

export const WordSummary: FC = () => {
    const dispatch = useDispatch();
    const word = useSelector(selectSelectedWord)?.wordBaseForm;
    useEffect(() => {
        dispatch(
            fetchWordIfNeeded({
                baseForm: word,
                pagingInfo: {
                    max: 40
                }
            })
        );
    });

    const context = useAppSelector(selectWordInfo.bind(null, word));
    if (!word) {
        return null;
    }

    return (
        <div className={styles.container}>
            <WordDefinition
                baseForm={word}
                exact
                initiallyOpen
                toggleDefinition
            />
            <Separator />
            <WordDefinition baseForm={word} initiallyOpen={false} />
            <Separator />
            <Drawer summary="Occurences">
                {context?.occurrences.map((con) => (
                    <div
                        className={styles.dialog}
                        key={con.episodeName + con.time}
                    >
                        <Dialog
                            readOnly
                            episode={con.episodeId}
                            time={con.time}
                            title={con.episodeName}
                        >
                            <WordLink
                                word={word}
                                includeEpisode={con.episodeId}
                                includeTime={con.time}
                                iconSize={1}
                            />
                        </Dialog>
                    </div>
                ))}
            </Drawer>
            <Separator />
        </div>
    );
};
