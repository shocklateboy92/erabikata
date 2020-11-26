import { mdiShare, mdiShareVariant, mdiVlc } from '@mdi/js';
import Icon from '@mdi/react';
import { useAppSelector } from 'app/hooks';
import { Drawer } from 'components/drawer';
import { Separator } from 'components/separator';
import { DialogList } from 'features/dialog/dialogList';
import { EngDialogList } from 'features/engDialog/engDialogList';
import { HassPlayButton } from 'features/hass';
import { WordDefinition } from 'features/wordDefinition';
import React, { FC } from 'react';
import { Link } from 'react-router-dom';
import styles from './selectedWord.module.scss';
import { selectSelectedWordInfo } from './slice';
import { WordLink } from './wordLink';

const ICON_SIZE = 1;

// `share()` isn't in the spec
declare global {
    interface Navigator {
        share(args: { text: string }): Promise<void>;
    }
}

export const SelectedWord: FC<{}> = () => {
    const wordInfo = useAppSelector(selectSelectedWordInfo);
    if (!wordInfo) {
        return null;
    }

    const parentEpisodeId = wordInfo.episode;
    const parentDialogId = wordInfo.sentence.startTime;

    const vlcCommand = wordInfo.context?.occurrences.find(
        (w) => w.time === parentDialogId && w.episodeId === parentEpisodeId
    )?.vlcCommand;

    const dialogUrl = new URL('/dialog', document.baseURI);
    dialogUrl.searchParams.set('episode', parentEpisodeId);
    dialogUrl.searchParams.set('time', parentDialogId.toString());

    return (
        <div>
            <div className={styles.summary}>
                <div className={styles.title}>{wordInfo.word}</div>
                <div className={styles.content}>
                    <div className={styles.text}>
                        Rank: {wordInfo.context?.rank ?? '????'}
                        <br />
                        Occurrences:{' '}
                        {wordInfo.context?.totalOccurrences ?? '????'}
                    </div>
                    <div className={styles.actions}>
                        <button
                            onClick={async () => {
                                const text = `[${wordInfo.word}](${dialogUrl}) #Japanese`;

                                console.log(`Sharing '${text}'...`);
                                await navigator.share({
                                    text
                                });
                            }}
                        >
                            <Icon path={mdiShareVariant} size={ICON_SIZE} />
                        </button>
                        <WordLink
                            word={wordInfo.word}
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
                <WordDefinition exact baseForm={wordInfo.word} initiallyOpen />
                <Separator />
                <WordDefinition
                    baseForm={wordInfo.word}
                    initiallyOpen={false}
                />
                <Separator />
                <Drawer
                    summary="Dialog Context"
                    extraActions={(iconSize) => (
                        <Link
                            to={{
                                pathname: '/dialog',
                                search: dialogUrl.search
                            }}
                        >
                            <Icon path={mdiShare} size={iconSize} />
                        </Link>
                    )}
                >
                    {
                        <DialogList
                            episode={wordInfo.episode}
                            time={wordInfo.sentence.startTime}
                            count={2}
                        />
                    }
                </Drawer>
                <Separator />
                <Drawer summary="English Context">
                    <EngDialogList
                        episodeId={wordInfo.episode}
                        time={wordInfo.sentence.startTime}
                    ></EngDialogList>
                </Drawer>
                <Separator />
            </div>
        </div>
    );
};
