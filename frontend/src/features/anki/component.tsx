import { FC } from 'react';
import { useTypedSelector } from 'app/hooks';
import { useSelector } from 'react-redux';
import {
    selectSelectedEpisodeId,
    selectSelectedTime,
    selectSelectedWords
} from 'features/selectedWord';

export const Anki: FC = () => {
    const isWordSelected = useTypedSelector(
        (state) => selectSelectedWords(state).length > 0
    );
    const isSentenceSelected = useTypedSelector(
        (state) =>
            selectSelectedEpisodeId(state) &&
            selectSelectedTime(state) !== undefined
    );
};

const SentenceField: FC = () => {
    const text = useTypedSelector((state) => state.anki.selectAnalyzer());
};
