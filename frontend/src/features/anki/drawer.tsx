import { mdiArrowRight } from '@mdi/js';
import Icon from '@mdi/react';
import { useTypedSelector } from 'app/hooks';
import { useAppDispatch } from 'app/store';
import { useWordsNotesQuery } from 'backend';
import { Pill } from 'components/pill';
import { QueryPlaceholder } from 'components/placeholder/queryPlaceholder';
import { Drawer } from 'features/drawer';
import { Ruby } from 'features/furigana';
import { selectSelectedWords, wordSelectionV2 } from 'features/selectedWord';
import { FC, PropsWithChildren } from 'react';
import './drawer.scss';

export const AnkiDrawer: FC<PropsWithChildren<{}>> = () => {
    return (
        <Drawer summary="Anki Notes">
            <DrawerContent />
        </Drawer>
    );
};

const DrawerContent: FC = () => {
    const dispatch = useAppDispatch();
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
            {data.map((note) => (
                <div className="anki-note-info" key={note.id}>
                    <div>
                        <ruby>
                            {note.primaryWord}
                            <rt>{note.primaryWordReading}</rt>
                        </ruby>
                        {note.primaryWord && note.words.length > 0 && (
                            <Icon path={mdiArrowRight} />
                        )}
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
                                onClick={() => {
                                    dispatch(
                                        wordSelectionV2(word.definitionIds)
                                    );
                                }}
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
