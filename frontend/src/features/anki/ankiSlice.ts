import { createSlice, PayloadAction } from '@reduxjs/toolkit';

export interface IEpisodeTime {
    episodeId: string;
    time: number;
}

interface IAnkiState {
    sentence?: IEpisodeTime;
    meaning?: IEpisodeTime;
    word: { id?: number; definitions: boolean[][] };
    image?: IEpisodeTime;
}

const initialState: IAnkiState = { word: { definitions: [] } };

const slice = createSlice({
    name: 'anki',
    initialState,
    reducers: {
        wordMeaningCheckToggle: (
            { word: { definitions, ...word }, ...state },
            {
                payload: { meaningIndex, senseIndex }
            }: PayloadAction<{ meaningIndex: number; senseIndex: number }>
        ) => ({
            ...state,
            word: {
                ...word,
                definitions: Object.assign([...definitions], {
                    [meaningIndex]: Object.assign(
                        [...(definitions[meaningIndex] ?? [])],
                        {
                            [senseIndex]: !(definitions[meaningIndex] ?? [])[
                                senseIndex
                            ]
                        }
                    )
                })
            }
        }),
        ankiSendCompletion: () => initialState,
        ankiTimeLockRequest: (
            state,
            {
                payload: { field, time }
            }: PayloadAction<{
                time: IEpisodeTime;
                field: 'sentence' | 'meaning' | 'image';
            }>
        ) => {
            const stateTime = state[field];
            return {
                ...state,
                [field]:
                    stateTime?.time === time.time &&
                    stateTime?.episodeId === time.episodeId
                        ? undefined
                        : time
            };
        }
    }
});

export const ankiReducer = slice.reducer;
export const {
    wordMeaningCheckToggle,
    ankiSendCompletion,
    ankiTimeLockRequest
} = slice.actions;
