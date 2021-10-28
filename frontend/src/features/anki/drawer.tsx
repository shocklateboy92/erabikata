import { useTypedSelector } from 'app/hooks';
import { useWordsNotesQuery } from 'backend';
import { Pill } from 'components/pill';
import { QueryPlaceholder } from 'components/placeholder/queryPlaceholder';
import { Drawer } from 'features/drawer';
import { selectSelectedWords } from 'features/selectedWord';
import { FC } from 'react';

export const AnkiDrawer: FC = () => {
    return (
        <Drawer summary="Anki Notes">
            <DrawerContent />
        </Drawer>
    );
};

const DrawerContent: FC = () => {
    const wordId = useTypedSelector((state) => selectSelectedWords(state)[0]);
    const { data, ...response } = useWordsNotesQuery(
        { wordId },
        { skip: !wordId }
    );

    if (typeof data === 'undefined') {
        return <QueryPlaceholder result={response} />;
    }
    return (
        <>
            {data.map((note, index) => (
                <div className="anki-note-info" key={index}>
                    <Pill>{'nid:' + note.id}</Pill>
                </div>
            ))}
        </>
    );
};
