import { mdiSend, mdiSync } from '@mdi/js';
import { useAppSelector, useTypedSelector } from 'app/hooks';
import { useAppDispatch } from 'app/store';
import { useEngSubsIndexQuery, useExecuteActionMutation } from 'backend';
import { EngSubsIndexApiArg } from 'backend.generated';
import { ActionButton } from 'components/button/actionButton';
import { Row } from 'components/layout';
import { QueryPlaceholder } from 'components/placeholder/queryPlaceholder';
import { useNearbyDialogQuery } from 'features/dialog/api';
import { Dialog } from 'features/dialog/Dialog';
import { EngDialog } from 'features/engDialog/engDialog';
import { ImageContext } from 'features/imageContext/component';
import { selectSelectedEpisodeTime } from 'features/selectedWord';
import React, { FC, Fragment, useState } from 'react';
import { useDispatch } from 'react-redux';
import { ankiTimeLockRequest, wordMeaningCheckToggle } from './ankiSlice';
import { sendToAnki, syncAnkiActivity } from './api';
import { ActionFieldView, FieldView } from './fieldView';
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
    const sentenceTime = useAppSelector(selectSentenceTimeToSend);
    const meaningTime = useAppSelector(selectMeaningTimeToSend);

    return (
        <>
            {sentenceTime && <SentenceField {...sentenceTime} />}
            {meaningTime && <MeaningField {...meaningTime} />}
            <ImageField />
            <WordField />
            <PrimaryWordNotesField />
            <LinkField />
            <ActionFieldView>
                <Row centerChildren>
                    <SendToAnkiButton />
                    <SyncAnkiButton />
                </Row>
            </ActionFieldView>
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
        <TimeLockableField title="Sentence" field="sentence">
            {!dialog ? (
                <QueryPlaceholder result={response} />
            ) : (
                <>
                    <Dialog dialogId={dialog.dialogId} compact />
                    {textToSend}
                </>
            )}
        </TimeLockableField>
    );
};

const MeaningField: FC<EngSubsIndexApiArg> = (args) => {
    const response = useEngSubsIndexQuery(args);
    return (
        <TimeLockableField title="Meaning" field="meaning">
            {!response.data ? (
                <QueryPlaceholder result={response} />
            ) : (
                <EngDialog compact content={response.data.dialog[0]} />
            )}
        </TimeLockableField>
    );
};

const WordField: FC = () => {
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
            <FieldView title="Primary word">
                <ruby>{kanji}</ruby>
            </FieldView>
            {reading && (
                <FieldView title="Primary word reading">
                    <ruby>{reading}</ruby>
                </FieldView>
            )}
            <FieldView title="Primary word meaning">
                <form>
                    {english.map((meaning, meaningIndex) => (
                        <Fragment key={meaningIndex}>
                            {meaning.senses.map(function (sense, senseIndex) {
                                const id = `some-unique-${meaningIndex}-${senseIndex}`;
                                return (
                                    <Fragment key={senseIndex}>
                                        <input
                                            id={id}
                                            type="checkbox"
                                            checked={
                                                !(uncheckedSenses[
                                                    meaningIndex
                                                ] ?? [])[senseIndex]
                                            }
                                            onChange={() => {
                                                dispatch(
                                                    wordMeaningCheckToggle({
                                                        meaningIndex,
                                                        senseIndex
                                                    })
                                                );
                                            }}
                                        />
                                        <label key={senseIndex} htmlFor={id}>
                                            {sense}
                                        </label>
                                    </Fragment>
                                );
                            })}
                        </Fragment>
                    ))}
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

const ImageField: FC = () => {
    const imageArgs = useAppSelector(selectImageTimeToSend);
    if (!imageArgs) {
        return null;
    }

    return (
        <TimeLockableField title="Image" field="image">
            <ImageContext includeSubs {...imageArgs} />
        </TimeLockableField>
    );
};

const LinkField: FC = () => {
    const link = useTypedSelector(selectSentenceLinkToSend);
    if (!link) {
        return null;
    }
    return <FieldView title="Erabikata Link">{link}</FieldView>;
};

const SyncAnkiButton: FC = () => {
    const [executeAction, { isLoading }] = useExecuteActionMutation();
    return (
        <ActionButton
            large
            isLoading={isLoading}
            icon={mdiSync}
            onClick={() => {
                executeAction(syncAnkiActivity());
            }}
        >
            Sync Anki
        </ActionButton>
    );
};

const SendToAnkiButton: FC = () => {
    const dispatch = useAppDispatch();
    const [isLoading, setLoading] = useState(false);
    return (
        <ActionButton
            large
            isLoading={isLoading}
            icon={mdiSend}
            onClick={async () => {
                setLoading(true);
                await dispatch(sendToAnki());
                setLoading(false);
            }}
        >
            Send to Anki
        </ActionButton>
    );
};

const TimeLockableField: FC<{
    field: 'image' | 'meaning' | 'sentence';
    title: string;
}> = ({ field, ...props }) => {
    const dispatch = useDispatch();
    const isActive = useTypedSelector((state) => !!state.anki[field]);
    const time = useTypedSelector(
        (state) => state.anki[field] ?? selectSelectedEpisodeTime(state)
    );
    if (!time) {
        return null;
    }

    return (
        <FieldView
            active={isActive}
            toggleActive={() => {
                dispatch(ankiTimeLockRequest({ field, time }));
            }}
            {...props}
        ></FieldView>
    );
};
