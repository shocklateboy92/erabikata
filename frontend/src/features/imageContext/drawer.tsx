import React, { FC } from 'react';
import { useTypedSelector } from '../../app/hooks';
import { selectSelectedWord } from '../selectedWord';
import { ImageContext } from './component';

export const ImageContextDrawer: FC = () => {
    const { episode, sentenceTimestamp } = useTypedSelector(selectSelectedWord);
    if (!(episode && sentenceTimestamp)) {
        return null;
    }

    return <ImageContext time={sentenceTimestamp} episodeId={episode} />;
};