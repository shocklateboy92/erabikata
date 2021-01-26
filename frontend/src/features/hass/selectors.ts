import { RootState } from 'app/rootReducer';
import { IPlayerInfo } from './slice';

export const selectPlayerList = (state: RootState) => state.hass.players;
export const selectIsPlayerSelected = (state: RootState) =>
    selectSelectedPlayerId(state) !== null;
export const selectSelectedPlayerId = (state: RootState) =>
    state.hass.selectedPlayer;

export const selectSelectedPlayer = (
    state: RootState
): IPlayerInfo | undefined => state.hass.players[state.hass.selectedPlayer!];

export const selectNowPlayingEpisodeId = (state: RootState) =>
    selectSelectedPlayer(state)?.media?.id;

export const selectIsCurrentPlayerActive = (state: RootState) =>
    state.hass.selectedPlayer &&
    state.hass.players[state.hass.selectedPlayer].state === 'playing';

export const selectIsPlayingInSelectedPlayer = (
    state: RootState,
    episodeId?: string
) => selectSelectedPlayer(state)?.media?.id.toString() === episodeId;
