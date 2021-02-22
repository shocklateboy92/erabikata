import { RootState } from '../../app/rootReducer';
import {
    selectEnglishDialogContent,
    selectNearbyEnglishDialog
} from '../engDialog/slice';

export const selectSelectedEnglishDialog = (state: RootState) => {
    const { episode, sentenceTimestamp } = state.selectedWord;
    if (!episode || !sentenceTimestamp) {
        return;
    }

    const nearest = selectNearbyEnglishDialog(
        state,
        episode,
        sentenceTimestamp,
        1
    );
    if (nearest.length < 1) {
        return;
    }

    return selectEnglishDialogContent(state, episode, nearest[0]);
};