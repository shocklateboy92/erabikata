import { useTypedSelector } from 'app/hooks';
import { isKana, selectIsFuriganaHiddenForWords } from 'features/furigana';
import React, { FC } from 'react';
import styles from './wordDefinition.module.scss';
import { Pill } from '../../components/pill';
import { Link } from 'react-router-dom';
import { useWordsKnownQuery, useWordsUnknownRanksQuery } from 'backend';
import { selectWordDefinition } from './selectors';
import { useDispatch } from 'react-redux';
import { wordPromotion } from 'features/selectedWord';

const PRIMARY_ELEMENT_ID = 'primary-word-definition-view';

export const Definition: FC<{
    wordId: number;
    episodeId?: string;
    isPrimary?: boolean;
}> = ({ wordId, episodeId, isPrimary }) => {
    const dispatch = useDispatch();
    const definition = useTypedSelector((state) =>
        selectWordDefinition(state, wordId)
    );
    const isReadingKnown = useTypedSelector((state) =>
        selectIsFuriganaHiddenForWords(state, [wordId])
    );
    const readingsOnly = useTypedSelector(
        (state) => state.wordDefinitions.readingsOnly
    );

    const { isKnown } = useWordsKnownQuery(
        {},
        {
            selectFromResult: (result) => ({
                isKnown: result.data?.[wordId]
            })
        }
    );

    const { unknownRank } = useWordsUnknownRanksQuery(
        {},
        {
            selectFromResult: (result) => ({
                unknownRank: result.data?.[wordId]
            })
        }
    );

    if (!definition) {
        return null;
    }

    const priorities = definition.priorities;

    return (
        <div className={styles.definition}>
            <div
                id={isPrimary ? PRIMARY_ELEMENT_ID : undefined}
                className={styles.title}
                onClick={() => {
                    if (isPrimary) {
                        return;
                    }
                    dispatch(wordPromotion(wordId));
                    document
                        .getElementById(PRIMARY_ELEMENT_ID)
                        ?.scrollIntoView();
                }}
            >
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
            {unknownRank && <Pill>Unkown {unknownRank}</Pill>}
            {priorities.news && <Pill>News</Pill>}
            {priorities.ichi && <Pill>Ichimango</Pill>}
            {priorities.gai && <Pill>Loanword</Pill>}
            {priorities.spec && <Pill>Special</Pill>}
            {priorities.freq && <Pill>General</Pill>}
            {isKnown && <Pill>Known {'A'}</Pill>}
            {isReadingKnown && <Pill>Known {'R'}</Pill>}
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
