import React, { FC } from 'react';
import { useTypedSelector } from '../../app/hooks';
import { selectSelectedWords } from '../selectedWord';
import { Drawer } from '../../components/drawer';
import { Link } from 'react-router-dom';
import Icon from '@mdi/react';
import { mdiShare } from '@mdi/js';
import { WordOccurrences } from './occurrences';

export const OccurrencesDrawer: FC = () => {
    const [wordId] = useTypedSelector(selectSelectedWords);
    if (!wordId) {
        return null;
    }
    return (
        <Drawer
            summary="Occurrences"
            extraActions={(iconSize) => (
                <Link to={`/ui/word/${wordId}`}>
                    <Icon path={mdiShare} size={iconSize} />
                </Link>
            )}
        >
            <WordOccurrences wordId={wordId} readOnly />
        </Drawer>
    );
};
