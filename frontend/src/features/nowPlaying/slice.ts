import { createSlice, PayloadAction, compose } from '@reduxjs/toolkit';
import { NowPlayingInfo, SubsClient } from 'backend.generated';
import { AppThunk } from 'app/store';
import { getToken } from 'api/plexToken';
import { RootState } from 'app/rootReducer';
import { osrStart, osrEnd } from 'features/spinnerTop';
import { notUndefined } from 'typeUtils';
import { selectSelectedPlayer } from 'features/hass';

interface INowPlayingState {
    pending: boolean;
    sessions: { [key: string]: Omit<NowPlayingInfo, 'dialog'> | undefined };
}

const initialState: INowPlayingState = {
    pending: false,
    sessions: {}
};

const slice = createSlice({
    name: 'nowPlaying',
    initialState,
    reducers: {
        nowPlayingFetchStarted: {
            reducer: (state) => ({ ...state, pending: true }),
            prepare: osrStart
        },
        nowPlayingFetchFailed: {
            reducer: (state) => ({ ...state, pending: false }),
            prepare: osrEnd
        },
        nowPlayingFetchSucceded: {
            reducer: (state, { payload }: PayloadAction<NowPlayingInfo[]>) => ({
                ...state,
                pending: false,
                sessions: payload.reduce<INowPlayingState['sessions']>(
                    (prev, { episodeId, time, episodeTitle }) => {
                        prev[episodeId] = {
                            episodeId,
                            time,
                            episodeTitle
                        };
                        return prev;
                    },
                    {}
                )
            }),
            prepare: (_) => osrEnd<NowPlayingInfo[]>(_)
        }
    }
});

export const nowPlayingFetchSucceded = slice.actions.nowPlayingFetchSucceded;

export const fetchNowPlayingSessions: () => AppThunk = () => (dispatch) => {
    dispatch(slice.actions.nowPlayingFetchStarted(undefined));

    new SubsClient()
        .nowPlaying(getToken())
        .then(compose(dispatch, slice.actions.nowPlayingFetchSucceded))
        .catch(compose(dispatch, slice.actions.nowPlayingFetchFailed));
};

export const selectNowPlayingSessions = (state: RootState) =>
    Object.values(state.nowPlaying.sessions).filter(notUndefined);

export const selectNowPlayingSessionsPending = (state: RootState) =>
    state.nowPlaying.pending;

export const selectIsPlayingInSelectedPlayer = (
    state: RootState,
    episodeId: string
) =>
    selectSelectedPlayer(state) &&
    !!selectNowPlayingSessions(state).find((s) => s.episodeId === episodeId);

export const nowPlayingReducer = slice.reducer;
