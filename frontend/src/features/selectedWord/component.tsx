import { useTypedSelector } from 'app/hooks';
import { Separator } from 'components/separator';
import { OccurrencesDrawer } from 'features/wordContext';
import { WordDefinition } from 'features/wordDefinition';
import React, { FC } from 'react';
import { DialogDrawer } from '../dialog/DialogDrawer';
import { selectSelectedWord, shouldShowPanel } from './selectors';
import { ImageContextDrawer } from '../imageContext/drawer';
import { EngDialogDrawer } from '../engDialog/drawer';

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
            <Separator />
            <DialogDrawer />
            <Separator />
            <ImageContextDrawer />
            <Separator />
            <EngDialogDrawer />
            <Separator />
            <OccurrencesDrawer />
            <Separator />
        </div>
    );
};
