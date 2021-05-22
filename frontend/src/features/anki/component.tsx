import { useAppSelector, useTypedSelector } from 'app/hooks';
import { useEngSubsIndexQuery } from 'backend';
import { QueryPlaceholder } from 'components/placeholder/queryPlaceholder';
import { useNearbyDialogQuery } from 'features/dialog/api';
import { Dialog } from 'features/dialog/Dialog';
import { EngDialog } from 'features/engDialog/engDialog';
import { ImageContext } from 'features/imageContext/component';
import {
    selectSelectedEpisodeTime,
    selectSelectedWords
} from 'features/selectedWord';
import { selectWordDefinition } from 'features/wordDefinition/selectors';
import { FC, Fragment } from 'react';
import { useDispatch } from 'react-redux';
import { IEpisodeTime, wordMeaningCheckToggle } from './ankiSlice';
import { FieldView } from './fieldView';

export const Anki: FC = () => {
    const sentenceTime = useAppSelector(
        (state) => state.anki.sentence ?? selectSelectedEpisodeTime(state)
    );
    const meaningTime = useAppSelector(
        (state) => state.anki.meaning ?? selectSelectedEpisodeTime(state)
    );
    const imageTime = useAppSelector(
        (state) => state.anki.image ?? selectSelectedEpisodeTime(state)
    );

    const wordId = useTypedSelector(
        (state) => state.anki.word?.id ?? selectSelectedWords(state)[0]
    );

    return (
        <>
            {sentenceTime && <SentenceField {...sentenceTime} />}
            {meaningTime && <MeaningField {...meaningTime} />}
            {imageTime && <ImageContext {...imageTime} />}
            {wordId && <WordKanjiField wordId={wordId} />}
        </>
    );
};

const SentenceField: FC<{ episodeId: string; time: number }> = ({
    episodeId,
    time
}) => {
    const {
        dialog: [dialog],
        response
    } = useNearbyDialogQuery(episodeId, time, 1);
    if (!dialog) {
        return <QueryPlaceholder result={response} />;
    }

    return <Dialog dialogId={dialog.dialogId} compact />;
};

const MeaningField: FC<IEpisodeTime> = ({ episodeId, time }) => {
    const response = useEngSubsIndexQuery({ episodeId, time, count: 0 });
    if (!response.data) {
        return <QueryPlaceholder result={response} />;
    }

    return <EngDialog compact content={response.data.dialog[0]} />;
};

const WordKanjiField: FC<{ wordId: number }> = ({ wordId }) => {
    const dispatch = useDispatch();
    const uncheckedSenses = useTypedSelector(
        (state) => state.anki.word?.definitions
    );
    const definition = useTypedSelector((state) =>
        selectWordDefinition(state, wordId)
    );
    if (!definition) {
        return null;
    }

    const {
        japanese: [{ kanji, reading }],
        english
    } = definition;
    return (
        <>
            <FieldView title="Primary word">{kanji}</FieldView>
            {reading && (
                <FieldView title="Primary word reading">{reading}</FieldView>
            )}
            <FieldView title="Primary word meaning">
                <form>
                    {english.map((meaning, index) => {
                        const id = 'some-unique-' + index;
                        return (
                            <div key={index} className="definitionSelect">
                                <input
                                    id={id}
                                    type="checkbox"
                                    checked={!uncheckedSenses[index]}
                                    onChange={() => {
                                        dispatch(wordMeaningCheckToggle(index));
                                    }}
                                />
                                {meaning.senses.map((sense, index) => (
                                    <label key={index} htmlFor={id}>
                                        {sense}
                                    </label>
                                ))}
                            </div>
                        );
                    })}
                </form>
            </FieldView>
            <FieldView title="Primary word notes">
                {english.map((meaning) => meaning.tags.join('; '))}
            </FieldView>
        </>
    );
};
