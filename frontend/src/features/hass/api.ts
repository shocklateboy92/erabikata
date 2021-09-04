import {
    AnyAction,
    compose,
    createAsyncThunk,
    ThunkDispatch
} from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';
import * as hass from 'home-assistant-js-websocket';
import React, { useContext } from 'react';
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
    async (context: IHassContext, { dispatch }) => {
        if (!context.connection) {
            context.connection = await createConnection(dispatch);
        }

        const states = await hass.getStates(context.connection);
        return Object.fromEntries(states.map((e) => [e.entity_id, e]));
    }
);

export const pause = createAsyncThunk(
    'hass/pause',
    async (context: IHassContext, { getState, dispatch }) => {
        const state = getState() as RootState;
        const entity_id = withPlayerOverrides(selectSelectedPlayerId(state));
        if (!entity_id) {
            return;
        }

        if (!context.connection) {
            context.connection = await createConnection(dispatch);
        }

        await hass.callService(context.connection, DOMAIN, 'media_pause', {
            entity_id
        });
    }
);

const DOMAIN = 'media_player';
export const playFrom = createAsyncThunk(
    'hass/playFrom',
    async (
        args: { context: IHassContext; timeStamp: number },
        { getState, dispatch }
    ) => {
        if (!args.context.connection) {
            args.context.connection = await createConnection(dispatch);
        }

        const state = getState();
        // TODO: try making this automagic by wrapping `createAsyncThunk`
        //       with a function that passes through everything, but sets
        //       the 3rd type argument to `RootState`.
        const entity_id = withPlayerOverrides(
            selectSelectedPlayerId(state as RootState)
        );

        await hass.callService(args.context.connection, DOMAIN, 'media_seek', {
            entity_id,
            seek_position: args.timeStamp
        });
        await hass.callService(args.context.connection, DOMAIN, 'media_play', {
            entity_id
        });
    }
);

// As of 2021-09-04, the plex integration seems to control apollo
const withPlayerOverrides = (entity_id: string | null) => entity_id;

interface IHassContext {
    connection?: hass.Connection;
}
export const HassContext = React.createContext<IHassContext>({});
export const useHass = () => useContext(HassContext);
