import { encodeSelectionParams } from '../selectedWord/api';

export const generateDialogLink = (
    episodeId: string,
    time: number,
    wordIds: number[]
) => `/dialog?${encodeSelectionParams(episodeId, time, wordIds)}`;
