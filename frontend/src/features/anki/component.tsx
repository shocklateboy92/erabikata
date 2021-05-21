import { useTypedSelector } from 'app/hooks';
import { QueryPlaceholder } from 'components/placeholder/queryPlaceholder';
import { useNearbyDialogQuery } from 'features/dialog/api';
import { Dialog } from 'features/dialog/Dialog';
import {
    selectSelectedEpisodeTime,
    selectSelectedWords
} from 'features/selectedWord';
import { FC } from 'react';

export const Anki: FC = () => {
    const isWordSelected = useTypedSelector(
        (state) => selectSelectedWords(state).length > 0
    );
    const selectedTime = useTypedSelector(
        (state) => state.anki.sentence ?? selectSelectedEpisodeTime(state)
    );

    return (
        <>
            {selectedTime && (
                <SentenceField
                    episodeId={selectedTime.episodeId}
                    time={selectedTime.time}
                />
            )}
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

    return <Dialog dialogId={dialog.dialogId} />;
};
