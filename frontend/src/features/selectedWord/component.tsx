import { useTypedSelector } from 'app/hooks';
import { Separator } from 'components/separator';
import { OccurrencesDrawer } from 'features/wordContext';
import { WordDefinitionDrawer } from 'features/wordDefinition';
import React, { FC } from 'react';
import { DialogDrawer } from '../dialog/DialogDrawer';
import { shouldShowPanel } from './selectors';
import { ImageContextDrawer } from '../imageContext/drawer';
import { EngDialogDrawer } from '../engDialog/drawer';

export const SelectedWord: FC = () => {
    if (!useTypedSelector(shouldShowPanel)) {
        return null;
    }

    return (
        <div>
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
            <OccurrencesDrawer />
            <Separator />
        </div>
    );
};
