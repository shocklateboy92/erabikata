import { createReducer, PayloadAction } from '@reduxjs/toolkit';
import { HassEntities } from 'home-assistant-js-websocket';
import {
    hassEntityUpdate,
    hassSocketDisconnection,
    hassSocketReady,
    playerSelection
} from './actions';
import { updatePlayerList } from './api';

export interface IPlayerInfo {
    id: PlayerId;
    name: string;
    media?: {
        id: number;
        title: string;
        position: number;
        position_last_updated_at: string;
    };
    state: string;
}

interface IHassState {
    players: {
        [key: string]: IPlayerInfo;
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

const overrides = {
    2985: 1185,
    2986: 1186,
    2987: 1187,
    2988: 1188,
    2989: 1189,
    2990: 1190,
    2991: 1191,
    2992: 1192,
    2993: 1193,
    2994: 1194,
    2995: 1195,
    2996: 1196,
    2997: 1197,
    2998: 1198
};
const checkOverrides = (id: keyof typeof overrides) => {
    if (id in overrides) {
        return overrides[id];
    }

    return id;
};

const entityUpdateReducer = (
    state: IHassState,
    { payload }: PayloadAction<HassEntities>
): IHassState => ({
    ...state,
    selectedPlayer:
        state.selectedPlayer! in payload &&
        payload[state.selectedPlayer!].state !== UNAVAILABLE_STATE
            ? state.selectedPlayer
            : null,
    players: Object.fromEntries([
        [null, { id: null, name: 'None', state: 'idle' }],
        ...Object.values(payload)
            .filter(
                (e) =>
                    e.entity_id.startsWith(PLEX_ID_PREFIX) &&
                    e.state !== UNAVAILABLE_STATE
            )
            .map((e): [string, IPlayerInfo] => [
                e.entity_id,
                {
                    name: e.attributes.friendly_name!,
                    id: e.entity_id,
                    state: e.state,
                    media: e.attributes[MEDIA_ID_KEY] && {
                        id: checkOverrides(e.attributes[MEDIA_ID_KEY]),
                        title: e.attributes['media_title'],
                        position: e.attributes['media_position'],
                        position_last_updated_at:
                            e.attributes['media_position_updated_at']
                    }
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
