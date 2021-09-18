import { useTypedSelector } from 'app/hooks';
import { Separator } from 'components/separator';
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
            <EngDialogDrawer />
            <Separator />
            <MediaControlsDrawer />
            <Separator />
            <OccurrencesDrawer />
            <Separator />
        </div>
    );
};
