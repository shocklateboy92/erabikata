import { mdiSend } from '@mdi/js';
import { useAppSelector, useTypedSelector } from 'app/hooks';
import { useEngSubsIndexQuery } from 'backend';
import { ActionButton } from 'components/button/actionButton';
import { QueryPlaceholder } from 'components/placeholder/queryPlaceholder';
import { useNearbyDialogQuery } from 'features/dialog/api';
import { Dialog } from 'features/dialog/Dialog';
import { EngDialog } from 'features/engDialog/engDialog';
import { ImageContext } from 'features/imageContext/component';
import { selectWordDefinition } from 'features/wordDefinition/selectors';
import { FC } from 'react';
import { useDispatch } from 'react-redux';
import { IEpisodeTime, wordMeaningCheckToggle } from './ankiSlice';
import { sendToAnki } from './api';
import { FieldView } from './fieldView';
import {
    selectImageTimeToSend,
    selectMeaningTimeToSend,
    selectSentenceLinkToSend,
    selectSentenceTextToSend,
    selectSentenceTimeToSend,
    selectWordIdToSend
} from './selectors';

export const Anki: FC = () => {
    const dispatch = useDispatch();
    const sentenceTime = useAppSelector(selectSentenceTimeToSend);
    const meaningTime = useAppSelector(selectMeaningTimeToSend);
    const imageTime = useAppSelector(selectImageTimeToSend);
    const wordId = useTypedSelector(selectWordIdToSend);

    return (
        <>
            {sentenceTime && <SentenceField {...sentenceTime} />}
            {meaningTime && <MeaningField {...meaningTime} />}
            {imageTime && <ImageContext {...imageTime} />}
            {wordId && <WordKanjiField wordId={wordId} />}
            <LinkField />
            <ActionButton
                icon={mdiSend}
                onClick={() => dispatch(sendToAnki())}
            ></ActionButton>
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
    const textToSend = useTypedSelector(selectSentenceTextToSend);

    return (
        <FieldView title="Sentence">
            {!dialog ? (
                <QueryPlaceholder result={response} />
            ) : (
                <>
                    <Dialog dialogId={dialog.dialogId} compact />
                    {textToSend}
                </>
            )}
        </FieldView>
    );
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

const LinkField: FC = () => (
    <FieldView title="Erabikata Link">
        {useTypedSelector(selectSentenceLinkToSend)}
    </FieldView>
);
