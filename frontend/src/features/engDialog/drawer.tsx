import React, { FC } from 'react';
import { useTypedSelector } from '../../app/hooks';
import { selectSelectedWord } from '../selectedWord';
import { Drawer } from '../../components/drawer';
import { EngDialogList } from './engDialogList';

export const EngDialogDrawer: FC = () => {
    const { episode, sentenceTimestamp } = useTypedSelector(selectSelectedWord);
    if (!(episode && sentenceTimestamp)) {
        return null;
    }
    return (
        <Drawer summary="English Context">
            <EngDialogList episodeId={episode} time={sentenceTimestamp} />
        </Drawer>
    );
};