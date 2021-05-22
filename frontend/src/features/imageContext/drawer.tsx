import { mdiSubtitlesOutline } from '@mdi/js';
import Icon from '@mdi/react';
import { InlineButton } from 'components/button';
import { Drawer } from 'components/drawer';
import React, { FC, useState } from 'react';
import { useTypedSelector } from '../../app/hooks';
import { selectSelectedWord } from '../selectedWord';
import { ImageContext } from './component';

export const ImageContextDrawer: FC = () => {
    const [includeSubs, setIncludeSubs] = useState(false);
    const { episode, sentenceTimestamp } = useTypedSelector(selectSelectedWord);
    if (!(episode && sentenceTimestamp)) {
        return null;
    }

    return (
        <Drawer
            summary="Image Context"
            extraActions={(iconSize) => (
                <InlineButton
                    onClick={() => {
                        setIncludeSubs(!includeSubs);
                    }}
                >
                    <Icon path={mdiSubtitlesOutline} size={iconSize} />
                </InlineButton>
            )}
        >
            <ImageContext
                time={sentenceTimestamp}
                episodeId={episode}
                includeSubs={includeSubs}
            />
        </Drawer>
    );
};

