import { useTypedSelector } from 'app/hooks';
import { Drawer } from 'components/drawer';
import { Separator } from 'components/separator';
import { EngDialogList } from 'features/engDialog/engDialogList';
import { ImageContext } from 'features/imageContext/component';
import { OccurrencesDrawer } from 'features/wordContext';
import { WordDefinition } from 'features/wordDefinition';
import React, { FC } from 'react';
import { DialogDrawer } from '../dialog/DialogDrawer';
import { selectSelectedWord, shouldShowPanel } from "./selectors";

export const SelectedWord: FC = () => {
    const {
        wordIds,
        episode: episodeId,
        sentenceTimestamp: dialogId
    } = useTypedSelector(selectSelectedWord);
    if (!useTypedSelector(shouldShowPanel)) {
        return null;
    }

    return (
        <div>
            <WordDefinition
                exact
                wordIds={wordIds}
                initiallyOpen
                toggleDefinition
            />
            <Separator />
            <WordDefinition wordIds={wordIds} initiallyOpen={false} />
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
                        <EngDialogList episodeId={episodeId} time={dialogId} />
                    </Drawer>
                </>
            )}
            <Separator />
            <OccurrencesDrawer />
            <Separator />
        </div>
    );
};
