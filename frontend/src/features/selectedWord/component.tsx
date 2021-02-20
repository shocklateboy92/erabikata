import { mdiClose, mdiShare } from '@mdi/js';
import Icon from '@mdi/react';
import { useTypedSelector } from 'app/hooks';
import { InlineButton } from 'components/button';
import { Drawer } from 'components/drawer';
import { Separator } from 'components/separator';
import { EngDialogList } from 'features/engDialog/engDialogList';
import { ImageContext } from 'features/imageContext/component';
import { WordContext } from 'features/wordContext';
import { WordOccurrences } from 'features/wordContext/occurrences';
import { WordDefinition } from 'features/wordDefinition';
import React, { FC } from 'react';
import { useDispatch } from 'react-redux';
import { Link } from 'react-router-dom';
import styles from './selectedWord.module.scss';
import { selectionClearRequest, selectSelectedWord } from './slice';
import { WordLink } from './wordLink';
import { DialogDrawer } from '../dialog/DialogDrawer';

const ICON_SIZE = 1;

export const SelectedWord: FC<{}> = () => {
    const dispatch = useDispatch();
    const selectedWord = useTypedSelector(selectSelectedWord);
    if (!selectedWord?.wordBaseForm) {
        return null;
    }

    const episodeId = selectedWord.episode;
    const dialogId = selectedWord.sentenceTimestamp;

    let dialogUrl: URL | null = null;
    if (episodeId && dialogId) {
        dialogUrl = new URL('/dialog', document.baseURI);
        dialogUrl.searchParams.set('episode', episodeId);
        dialogUrl.searchParams.set('time', dialogId.toString());
        dialogUrl.searchParams.set('word', selectedWord.wordBaseForm);
    }

    return (
        <div>
            <div className={styles.summary}>
                <div className={styles.title}>{selectedWord.wordBaseForm}</div>
                <div className={styles.content}>
                    <WordContext word={selectedWord.wordBaseForm} />
                    <div className={styles.actions}>
                        <WordLink
                            word={selectedWord.wordBaseForm}
                            includeEpisode={episodeId}
                            includeTime={dialogId}
                            iconSize={ICON_SIZE}
                        />
                        <InlineButton
                            hideOnMobile
                            onClick={() => {
                                dispatch(selectionClearRequest());
                            }}
                        >
                            <Icon path={mdiClose} size={ICON_SIZE} />
                        </InlineButton>
                    </div>
                </div>
            </div>
            <div className={styles.content}>
                <Separator />
                <WordDefinition
                    exact
                    wordIds={selectedWord.wordIds}
                    initiallyOpen
                    toggleDefinition
                />
                <Separator />
                <WordDefinition
                    wordIds={selectedWord.wordIds}
                    initiallyOpen={false}
                />
                {episodeId && dialogId && (
                    <>
                        <Separator />
                        <DialogDrawer />
                    </>
                )}
                {episodeId && dialogId && !isNaN(parseInt(episodeId)) && (
                    <>
                        <Separator />
                        <ImageContext episodeId={episodeId} time={dialogId} />
                    </>
                )}
                {episodeId && dialogId && (
                    <>
                        <Separator />
                        <Drawer summary="English Context">
                            <EngDialogList
                                episodeId={episodeId}
                                time={dialogId}
                            />
                        </Drawer>
                    </>
                )}
                <Separator />

                <Drawer
                    summary="Occurrences"
                    extraActions={(iconSize) => (
                        <Link to={`/word/${selectedWord.wordBaseForm}`}>
                            <Icon path={mdiShare} size={iconSize} />
                        </Link>
                    )}
                >
                    <WordOccurrences word={selectedWord.wordBaseForm} />
                </Drawer>
                <Separator />
            </div>
        </div>
    );
};
