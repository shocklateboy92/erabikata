import { createSelector } from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';
import { apiEndpoints } from 'backend';
import { IPlayerInfo } from './slice';

export const selectPlayerList = (state: RootState) => state.hass.players;
export const selectIsPlayerSelected = (state: RootState) =>
    selectSelectedPlayerId(state) !== null;
export const selectSelectedPlayerId = (state: RootState) =>
    state.hass.selectedPlayer;

const selectSelectedPlayerInternal = (
    state: RootState
): IPlayerInfo | undefined => state.hass.players[state.hass.selectedPlayer!];

export const selectSelectedPlayer = createSelector(
    selectSelectedPlayerInternal,
    apiEndpoints.alternateIdsMap.select(),
    (playerInfo, overridesMap) => {
        if (!playerInfo?.media) {
            return playerInfo;
        }

        const id = overridesMap.data?.[playerInfo.media.id];
        if (!id) {
            return playerInfo;
        }

        return {
            ...playerInfo,
            media: {
                ...playerInfo.media,
                id
            }
        };
    }
);

export const selectIsCurrentPlayerActive = (state: RootState) =>
    state.hass.selectedPlayer &&
    state.hass.players[state.hass.selectedPlayer].state === 'playing';

export const selectIsPlayingInSelectedPlayer = (
    state: RootState,
    episodeId?: string
) => selectSelectedPlayer(state)?.media?.id.toString() === episodeId;
