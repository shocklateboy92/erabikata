import { RootState } from '../../app/rootReducer';
import {
    selectEnglishDialogContent,
    selectNearbyEnglishDialog
} from '../engDialog/slice';
import { selectDialogContent } from '../dialog/slice';

export const selectSelectedWordContext = (state: RootState) =>
    (state.selectedWord?.wordBaseForm &&
        state.wordContexts.byId[state.selectedWord.wordBaseForm]) ||
    null;
export const selectSelectedDialog = (state: RootState) => {
    const { episode, sentenceTimestamp } = state.selectedWord;
    if (!(episode && sentenceTimestamp)) {
        return;
    }

    // const dialog = selectNearbyDialog(episode, sentenceTimestamp, 1, state);
    // if (dialog.length < 1) {
    //     return;
    // }

    return selectDialogContent(episode, sentenceTimestamp, state);
};
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