import { mdiSend } from '@mdi/js';
import { useAppSelector, useTypedSelector } from 'app/hooks';
import { useEngSubsIndexQuery } from 'backend';
import { EngSubsIndexApiArg } from 'backend-rtk.generated';
import { ActionButton } from 'components/button/actionButton';
import { QueryPlaceholder } from 'components/placeholder/queryPlaceholder';
import { useNearbyDialogQuery } from 'features/dialog/api';
import { Dialog } from 'features/dialog/Dialog';
import { EngDialog } from 'features/engDialog/engDialog';
import { ImageContext } from 'features/imageContext/component';
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
    selectWordDefinitionToSend,
    selectWordTagsToSend
} from './selectors';

export const Anki: FC = () => {
    const dispatch = useDispatch();
    const sentenceTime = useAppSelector(selectSentenceTimeToSend);
    const meaningTime = useAppSelector(selectMeaningTimeToSend);
    const imageTime = useAppSelector(selectImageTimeToSend);

    return (
        <>
            {sentenceTime && <SentenceField {...sentenceTime} />}
            {meaningTime && <MeaningField {...meaningTime} />}
            {imageTime && <ImageField {...imageTime} />}
            <WordKanjiField />
            <PrimaryWordNotesField />
            <LinkField />
            <ActionButton icon={mdiSend} onClick={() => dispatch(sendToAnki())}>
                Send to Anki
            </ActionButton>
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

const MeaningField: FC<EngSubsIndexApiArg> = (args) => {
    const response = useEngSubsIndexQuery(args);
    return (
        <FieldView title="Meaning">
            {!response.data ? (
                <QueryPlaceholder result={response} />
            ) : (
                <EngDialog compact content={response.data.dialog[0]} />
            )}
        </FieldView>
    );
};

const WordKanjiField: FC = () => {
    const dispatch = useDispatch();
    const uncheckedSenses = useTypedSelector(
        (state) => state.anki.word?.definitions
    );
    const definition = useTypedSelector(selectWordDefinitionToSend);
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
        </>
    );
};

const PrimaryWordNotesField: FC = () => {
    const tags = useAppSelector(selectWordTagsToSend);
    if (!tags) {
        return null;
    }

    return <FieldView title="Primary word notes">{tags}</FieldView>;
};

const ImageField: FC<IEpisodeTime> = (props) => (
    <FieldView title="Image">
        <ImageContext includeSubs {...props} />
    </FieldView>
);

const LinkField: FC = () => (
    <FieldView title="Erabikata Link">
        {useTypedSelector(selectSentenceLinkToSend)}
    </FieldView>
);
