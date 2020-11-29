import { createReducer, PayloadAction } from '@reduxjs/toolkit';
import { HassEntities } from 'home-assistant-js-websocket';
import {
    hassEntityUpdate,
    hassSocketDisconnection,
    hassSocketReady,
    playerSelection
} from './actions';
import { updatePlayerList } from './api';

interface IHassState {
    players: {
        [key: string]: {
            id: PlayerId;
            name?: string;
            mediaId?: number;
            state?: string;
        };
    };
    selectedPlayer: PlayerId;
    socketReady: boolean;
}
type PlayerId = string | null;

const initialState: IHassState = {
    players: {},
    selectedPlayer: null,
    socketReady: false
};

const PLEX_ID_PREFIX = 'media_player.plex_';
const MEDIA_ID_KEY = 'media_content_id';
const UNAVAILABLE_STATE = 'unavailable';

const entityUpdateReducer = (
    state: IHassState,
    { payload }: PayloadAction<HassEntities>
) => ({
    ...state,
    selectedPlayer:
        state.selectedPlayer! in payload &&
        payload[state.selectedPlayer!].state !== UNAVAILABLE_STATE
            ? state.selectedPlayer
            : null,
    players: Object.fromEntries([
        [null, { id: null, name: 'None' }],
        ...Object.values(payload)
            .filter(
                (e) =>
                    e.entity_id.startsWith(PLEX_ID_PREFIX) &&
                    e.state !== UNAVAILABLE_STATE
            )
            .map((e) => [
                e.entity_id,
                {
                    name: e.attributes.friendly_name,
                    id: e.entity_id,
                    state: e.state,
                    mediaId: e.attributes[MEDIA_ID_KEY]
                }
            ])
    ])
});

export const hassReducer = createReducer(initialState, (builder) =>
    builder
        .addCase(hassSocketDisconnection, (state) => ({
            ...state,
            socketReady: false
        }))
        .addCase(hassSocketReady, (state) => ({
            ...state,
            socketReady: true
        }))
        .addCase(hassEntityUpdate, entityUpdateReducer)
        .addCase(updatePlayerList.fulfilled, entityUpdateReducer)
        .addCase(playerSelection, (state, { payload }) => ({
            ...state,
            selectedPlayer: payload
        }))
);
