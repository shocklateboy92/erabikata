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
import { FC } from 'react';
import { IEpisodeTime } from './ankiSlice';

export const Anki: FC = () => {
    const isWordSelected = useTypedSelector(
        (state) => selectSelectedWords(state).length > 0
    );
    const sentenceTime = useAppSelector(
        (state) => state.anki.sentence ?? selectSelectedEpisodeTime(state)
    );
    const meaningTime = useAppSelector(
        (state) => state.anki.meaning ?? selectSelectedEpisodeTime(state)
    );
    const imageTime = useAppSelector(
        (state) => state.anki.image ?? selectSelectedEpisodeTime(state)
    );

    return (
        <>
            {sentenceTime && <SentenceField {...sentenceTime} />}
            {meaningTime && <MeaningField {...meaningTime} />}
            {imageTime && <ImageContext {...imageTime} />}
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
