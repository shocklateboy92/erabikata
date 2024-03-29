import { useTypedSelector } from 'app/hooks';
import { Separator } from 'components/separator';
import { AnkiDrawer } from 'features/anki/drawer';
import { MediaControlsDrawer } from 'features/mediaControls';
import { OccurrencesDrawer } from 'features/wordContext';
import { WordDefinitionDrawer } from 'features/wordDefinition';
import React, { FC } from 'react';
import { DialogDrawer } from '../dialog/DialogDrawer';
import { EngDialogDrawer } from '../engDialog/drawer';
import { ImageContextDrawer } from '../imageContext/drawer';
import './selectedWord.scss';
import { shouldShowPanel } from './selectors';

export const SelectedWord: FC = () => {
    if (!useTypedSelector(shouldShowPanel)) {
        return null;
    }

    return (
        <div className="selectedWord container">
            <WordDefinitionDrawer exact initiallyOpen toggleDefinition />
            <Separator />
            <WordDefinitionDrawer initiallyOpen={false} />
            <Separator />
            <DialogDrawer />
            <Separator />
            <ImageContextDrawer />
            <Separator />
            <MediaControlsDrawer />
            <Separator />
            <EngDialogDrawer />
            <Separator />
            <AnkiDrawer />
            <Separator />
            <OccurrencesDrawer />
            <Separator />
        </div>
    );
};
