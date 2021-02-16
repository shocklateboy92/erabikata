import { mdiPageNextOutline } from '@mdi/js';
import Icon from '@mdi/react';
import { useAppSelector } from 'app/hooks';
import { Drawer } from 'components/drawer';
import { isKana } from 'features/furigana';
import React, { FC, useEffect, useState } from 'react';
import { useDispatch } from 'react-redux';
import { fetchDefinitionsIfNeeded, selectDefinitionsById } from './slice';
import styles from './wordDefinition.module.scss';

export const WordDefinition: FC<{
    wordIds: number[];
    exact?: boolean;
    initiallyOpen: boolean;
    toggleDefinition?: boolean;
}> = ({ wordIds, initiallyOpen, exact, toggleDefinition }) => {
    const dispatch = useDispatch();
    useEffect(() => {
        dispatch(fetchDefinitionsIfNeeded(wordIds));
    }, [wordIds, dispatch]);

    const results = useAppSelector((state) =>
        selectDefinitionsById(state, wordIds)
    );

    const [showDefinition, setShowDefinition] = useState(true);

    const definition = exact ? results.slice(0, 1) : results.slice(1);
    return (
        <Drawer
            summary={exact ? 'Definition' : 'Related Words'}
            extraActions={(iconSize) =>
                toggleDefinition && (
                    <button onClick={() => setShowDefinition(!showDefinition)}>
                        <Icon path={mdiPageNextOutline} size={iconSize} />
                    </button>
                )
            }
            startOpen={initiallyOpen}
        >
            {definition.map((definition, index) => (
                <div key={index} className={styles.definition}>
                    <div className={styles.title}>
                        {definition.japanese.map((word, i) => (
                            <ruby key={i}>
                                {word.kanji}
                                {!isKana(word.kanji) && <rt>{word.reading}</rt>}
                            </ruby>
                        ))}
                    </div>
                    {/* {definition.isCommon && (
                        <span className={styles.commonTag}>Common word</span>
                    )} */}
                    {(!toggleDefinition || showDefinition) &&
                        definition.english.map((english, index) => (
                            <div key={index} className={styles.sense}>
                                {english.tags.length > 0 && (
                                    <div className={styles.tags}>
                                        {english.tags.join(', ')}
                                    </div>
                                )}
                                {english.senses.map((sense, index) => (
                                    <div key={index} className={styles.content}>
                                        {sense}
                                    </div>
                                ))}
                            </div>
                        ))}
                </div>
            ))}
        </Drawer>
    );
};
