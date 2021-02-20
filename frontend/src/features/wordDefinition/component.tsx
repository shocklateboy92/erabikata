import { mdiPageNextOutline } from '@mdi/js';
import Icon from '@mdi/react';
import { useTypedSelector } from 'app/hooks';
import { Drawer } from 'components/drawer';
import { isKana } from 'features/furigana';
import React, { FC, useEffect, useState } from 'react';
import { useDispatch } from 'react-redux';
import { fetchDefinitionsIfNeeded, fetchEpisodeRanksIfNeeded } from './slice';
import styles from './wordDefinition.module.scss';
import { selectSelectedEpisodeId, selectSelectedWords } from '../selectedWord';

const Definition: FC<{ wordId: number; episodeId?: string }> = ({
    wordId,
    episodeId
}) => {
    const definition = useTypedSelector(
        (state) => state.wordDefinitions.byId[wordId]
    );
    const episodeRank = useTypedSelector(
        (state) =>
            (state.wordDefinitions.episodeRanks[episodeId!] ?? {})[wordId]
    );

    if (!definition) {
        return null;
    }

    return (
        <div className={styles.definition}>
            <div className={styles.title}>
                {definition.japanese.map((word, i) => (
                    <ruby key={i}>
                        {word.kanji}
                        {!isKana(word.kanji) && <rt>{word.reading}</rt>}
                    </ruby>
                ))}
            </div>
            {/* TODO: Implement definition hiding */}
            {definition.english.map((english, index) => (
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
    );
};

export const WordDefinition: FC<{
    wordIds: number[];
    exact?: boolean;
    initiallyOpen: boolean;
    toggleDefinition?: boolean;
}> = ({ wordIds, initiallyOpen, exact, toggleDefinition }) => {
    const selectedEpisode = useTypedSelector(selectSelectedEpisodeId);

    const dispatch = useDispatch();
    useEffect(() => {
        dispatch(fetchDefinitionsIfNeeded(wordIds));
        selectedEpisode &&
            dispatch(fetchEpisodeRanksIfNeeded([selectedEpisode, wordIds]));
    }, [wordIds, selectedEpisode, dispatch]);

    const results = useTypedSelector(selectSelectedWords);

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
            {definition.map((word) => (
                <Definition wordId={word} key={word} />
            ))}
        </Drawer>
    );
};
