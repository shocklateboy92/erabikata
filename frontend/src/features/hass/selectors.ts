import { RootState } from 'app/rootReducer';

export const selectPlayerList = (state: RootState) => state.hass.players;
export const selectIsPlayerSelected = (state: RootState) =>
    selectSelectedPlayerId(state) !== null;
export const selectSelectedPlayerId = (state: RootState) =>
    state.hass.selectedPlayer;

export const selectSelectedPlayer = (state: RootState) =>
    state.hass.players[state.hass.selectedPlayer!];

export const isCurrentPlayerActive = (state: RootState) =>
    state.hass.selectedPlayer &&
    state.hass.players[state.hass.selectedPlayer].state === 'playing';
