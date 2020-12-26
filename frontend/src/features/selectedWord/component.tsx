import { mdiShare, mdiShareVariant, mdiVlc } from '@mdi/js';
import Icon from '@mdi/react';
import { useTypedSelector } from 'app/hooks';
import { Drawer } from 'components/drawer';
import { Separator } from 'components/separator';
import { DialogList } from 'features/dialog/dialogList';
import { EngDialogList } from 'features/engDialog/engDialogList';
import { HassPlayButton } from 'features/hass';
import { ImageContext } from 'features/imageContext/component';
import { WordDefinition } from 'features/wordDefinition';
import React, { FC } from 'react';
import { Link } from 'react-router-dom';
import styles from './selectedWord.module.scss';
import { selectSelectedWord, selectSelectedWordContext } from './slice';
import { WordLink } from './wordLink';

const ICON_SIZE = 1;

// `share()` isn't in the spec
declare global {
    interface Navigator {
        share(args: { text: string }): Promise<void>;
    }
}

export const SelectedWord: FC<{}> = () => {
    const selectedWord = useTypedSelector(selectSelectedWord);
    const wordContext = useTypedSelector(selectSelectedWordContext);
    if (!selectedWord) {
        return null;
    }

    const parentEpisodeId = selectedWord.episode;
    const parentDialogId = selectedWord.sentenceTimestamp;

    const vlcCommand = wordContext?.occurrences.find(
        (w) => w.time === parentDialogId && w.episodeId === parentEpisodeId
    )?.vlcCommand;

    let dialogUrl: URL | null = null;
    if (parentEpisodeId && parentDialogId) {
        dialogUrl = new URL('/dialog', document.baseURI);
        dialogUrl.searchParams.set('episode', parentEpisodeId);
        dialogUrl.searchParams.set('time', parentDialogId.toString());
    }

    return (
        <div>
            <div className={styles.summary}>
                <div className={styles.title}>{selectedWord.wordBaseForm}</div>
                <div className={styles.content}>
                    <div className={styles.text}>
                        Rank: {wordContext?.rank ?? '????'}
                        <br />
                        Occurrences: {wordContext?.totalOccurrences ?? '????'}
                    </div>
                    <div className={styles.actions}>
                        <button
                            onClick={async () => {
                                const text = `[${selectedWord.wordBaseForm}](${dialogUrl}) #Japanese`;

                                console.log(`Sharing '${text}'...`);
                                await navigator.share({
                                    text
                                });
                            }}
                        >
                            <Icon path={mdiShareVariant} size={ICON_SIZE} />
                        </button>
                        <WordLink
                            word={selectedWord.wordBaseForm}
                            includeEpisode={parentEpisodeId}
                            includeTime={parentDialogId}
                            iconSize={ICON_SIZE}
                        />
                        {vlcCommand && (
                            <button
                                onClick={() =>
                                    navigator.clipboard.writeText(vlcCommand)
                                }
                            >
                                <Icon path={mdiVlc} size={ICON_SIZE} />
                            </button>
                        )}
                        <HassPlayButton
                            dialogId={parentDialogId}
                            episodeId={parentEpisodeId}
                            iconSize={ICON_SIZE}
                        />
                    </div>
                </div>
            </div>
            <div className={styles.content}>
                <Separator />
                <WordDefinition
                    exact
                    baseForm={selectedWord.wordBaseForm}
                    initiallyOpen
                />
                <Separator />
                <WordDefinition
                    baseForm={selectedWord.wordBaseForm}
                    initiallyOpen={false}
                />
                {dialogUrl && (
                    <>
                        <Separator />
                        <Drawer
                            summary="Dialog Context"
                            extraActions={(iconSize) => (
                                <Link
                                    to={{
                                        pathname: '/dialog',
                                        search: dialogUrl!.search
                                    }}
                                >
                                    <Icon path={mdiShare} size={iconSize} />
                                </Link>
                            )}
                        >
                            {
                                <DialogList
                                    episode={parentEpisodeId!}
                                    time={parentDialogId!}
                                    count={2}
                                />
                            }
                        </Drawer>
                    </>
                )}
                {parentEpisodeId &&
                    parentDialogId &&
                    !isNaN(parseInt(parentEpisodeId)) && (
                        <>
                            <Separator />
                            <ImageContext
                                episodeId={parentEpisodeId}
                                time={parentDialogId}
                            />
                        </>
                    )}
                {parentEpisodeId && parentDialogId && (
                    <>
                        <Separator />
                        <Drawer summary="English Context">
                            <EngDialogList
                                episodeId={parentEpisodeId}
                                time={parentDialogId}
                            ></EngDialogList>
                        </Drawer>
                    </>
                )}
                <Separator />
            </div>
        </div>
    );
};
