import {
    AnyAction,
    compose,
    createAsyncThunk,
    ThunkDispatch
} from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';
import { apiEndpoints } from 'backend';
import * as hass from 'home-assistant-js-websocket';
import {
    hassEntityUpdate,
    hassSocketDisconnection,
    hassSocketReady
} from './actions';
import { selectSelectedPlayerId } from './selectors';

const STORAGE_KEY = 'hass_state';

const createConnection = async (
    dispatch: ThunkDispatch<unknown, unknown, AnyAction>
) => {
    const connection = await hass.createConnection({
        auth: await hass.getAuth({
            hassUrl: 'https://home-assistant.apps.lasath.org',
            saveTokens: (data) =>
                window.localStorage.setItem(STORAGE_KEY, JSON.stringify(data)),
            loadTokens: async () => {
                const data = window.localStorage.getItem(STORAGE_KEY);
                if (data) {
                    return JSON.parse(data);
                }
            }
        })
    });

    // Not doing fancy checks because `createConnection` will only be
    // called once (pending weird bugs).
    dispatch(apiEndpoints.alternateIdsMap.initiate({}));

    connection.addEventListener('ready', compose(dispatch, hassSocketReady));
    connection.addEventListener(
        'disconnected',
        compose(dispatch, hassSocketDisconnection)
    );
    connection.addEventListener(
        'reconnect-error',
        compose(dispatch, hassSocketDisconnection)
    );

    hass.subscribeEntities(connection, compose(dispatch, hassEntityUpdate));

    return connection;
};

export const updatePlayerList = createAsyncThunk(
    'hass/updatePlayers',
    async (_, { dispatch }) => {
        if (!globalContext.connection) {
            globalContext.connection = await createConnection(dispatch);
        }

        const states = await hass.getStates(globalContext.connection);
        return Object.fromEntries(states.map((e) => [e.entity_id, e]));
    }
);

export const togglePlayback = createAsyncThunk(
    'hass/media_play_pause',
    async (_, { getState, dispatch }) => {
        const state = getState() as RootState;
        const entity_id = withPlayerOverrides(selectSelectedPlayerId(state));
        if (!entity_id) {
            return;
        }

        if (!globalContext.connection) {
            globalContext.connection = await createConnection(dispatch);
        }

        await hass.callService(
            globalContext.connection,
            DOMAIN,
            'media_play_pause',
            {
                entity_id
            }
        );
    }
);

const DOMAIN = 'media_player';
export const playFrom = createAsyncThunk(
    'hass/playFrom',
    async (args: { timeStamp: number }, { getState, dispatch }) => {
        if (!globalContext.connection) {
            globalContext.connection = await createConnection(dispatch);
        }

        const state = getState();
        // TODO: try making this automagic by wrapping `createAsyncThunk`
        //       with a function that passes through everything, but sets
        //       the 3rd type argument to `RootState`.
        const entity_id = withPlayerOverrides(
            selectSelectedPlayerId(state as RootState)
        );

        await hass.callService(globalContext.connection, DOMAIN, 'media_seek', {
            entity_id,
            seek_position: args.timeStamp
        });
        await hass.callService(globalContext.connection, DOMAIN, 'media_play', {
            entity_id
        });
    }
);

// As of 2021-09-04, the plex integration seems to control apollo
const withPlayerOverrides = (entity_id: string | null) =>
    entity_id === 'media_player.plex_plex_for_android_tv_shield_android_tv'
        ? 'media_player.apollo'
        : entity_id;

interface IHassContext {
    connection?: hass.Connection;
}
const globalContext: IHassContext = {};
