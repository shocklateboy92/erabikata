import { mdiArrowRight } from '@mdi/js';
import Icon from '@mdi/react';
import { useTypedSelector } from 'app/hooks';
import { useWordsNotesQuery } from 'backend';
import { Pill } from 'components/pill';
import { QueryPlaceholder } from 'components/placeholder/queryPlaceholder';
import { Drawer } from 'features/drawer';
import { Ruby } from 'features/furigana';
import { selectSelectedWords } from 'features/selectedWord';
import { FC } from 'react';
import './drawer.scss';

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
                    <div>
                        <ruby>
                            {note.primaryWord}
                            <rt>{note.primaryWordReading}</rt>
                        </ruby>
                        <Icon path={mdiArrowRight} />
                        {note.words.map((word, index) => (
                            <Ruby
                                key={index}
                                wordIds={word.definitionIds}
                                known
                                active={
                                    !!word.definitionIds.find(
                                        (w) => w === wordId
                                    )
                                }
                            >
                                {word.displayText}
                            </Ruby>
                        ))}
                    </div>
                    <div className="nid">
                        <Pill>{'nid:' + note.id}</Pill>
                    </div>
                </div>
            ))}
        </>
    );
};
