import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';
import { AppThunk } from 'app/store';
import { IPlayerInfo, selectSelectedPlayer } from 'features/hass';

interface INowPlayingState {
    position: number;
}

const initialState: INowPlayingState = { position: 0 };

const slice = createSlice({
    name: 'nowPlaying',
    initialState,
    reducers: {
        nowPlayingPositionUpdateRequest: (
            _,
            {
                payload: { time, selectedPlayer }
            }: PayloadAction<{ time: string; selectedPlayer?: IPlayerInfo }>
        ) => {
            if (!selectedPlayer?.media) {
                return { position: 0 };
            }

            if (selectedPlayer.state === 'playing') {
                return {
                    position:
                        selectedPlayer.media.position +
                        (new Date(time).getTime() -
                            new Date(
                                selectedPlayer.media.position_last_updated_at
                            ).getTime()) /
                            1000
                };
            }

            return { position: selectedPlayer.media.position };
        }
    }
});

export const selectIsPlayingInSelectedPlayer = (
    state: RootState,
    episodeId: string
) =>
    state.hass.selectedPlayer &&
    state.hass.players[state.hass.selectedPlayer].media?.id.toString() ===
        episodeId;

export const selectNowPlayingMediaTimeStamp = (state: RootState) =>
    state.nowPlaying.position;

export const nowPlayingPositionUpdateRequest = (): AppThunk => (
    dispatch,
    getState
) =>
    dispatch(
        slice.actions.nowPlayingPositionUpdateRequest({
            time: new Date().toISOString(),
            selectedPlayer: selectSelectedPlayer(getState())
        })
    );

export const nowPlayingReducer = slice.reducer;
