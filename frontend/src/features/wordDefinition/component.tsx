import { useTypedSelector } from 'app/hooks';
import { isKana } from 'features/furigana';
import React, { FC } from 'react';
import styles from './wordDefinition.module.scss';
import { Pill } from '../../components/pill';
import { Link } from 'react-router-dom';
import { useWordsKnownQuery } from 'backend';
import { selectWordDefinition } from './selectors';

export const Definition: FC<{ wordId: number; episodeId?: string }> = ({
    wordId,
    episodeId
}) => {
    const definition = useTypedSelector((state) =>
        selectWordDefinition(state, wordId)
    );
    const episodeRank = useTypedSelector(
        (state) =>
            (state.wordDefinitions.episodeRanks[episodeId!] ?? {})[wordId]
    );
    const readingsOnly = useTypedSelector(
        (state) => state.wordDefinitions.readingsOnly
    );

    const { isKnown } = useWordsKnownQuery(
        {},
        {
            selectFromResult: (result) => ({
                isKnown: result.data?.includes(wordId)
            })
        }
    );

    if (!definition) {
        return null;
    }

    const priorities = definition.priorities;

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
            {definition.globalRank && (
                <Link to={`/ui/word/${wordId}?word=${wordId}`}>
                    <Pill>Global {definition.globalRank}</Pill>
                </Link>
            )}
            {episodeRank && <Pill>Episode {episodeRank}</Pill>}
            {priorities.news && <Pill>News</Pill>}
            {priorities.ichi && <Pill>Ichimango</Pill>}
            {priorities.gai && <Pill>Loanword</Pill>}
            {priorities.spec && <Pill>Special</Pill>}
            {priorities.freq && <Pill>General</Pill>}
            {isKnown && <Pill>Known {'A'}</Pill>}
            {!readingsOnly &&
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
    );
};
