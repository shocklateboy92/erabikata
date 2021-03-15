import {
    AnyAction,
    compose,
    createAsyncThunk,
    ThunkDispatch
} from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';
import axios from 'axios';
import * as hass from 'home-assistant-js-websocket';
import React, { useContext } from 'react';
import {
    hassEntityUpdate,
    hassSocketDisconnection,
    hassSocketReady
} from './actions';
import { selectSelectedPlayerId } from './selectors';

const STORAGE_KEY = 'hass_state';
const hassUrl = 'https://home-assistant.apps.lasath.org';

const getAuth = () =>
    hass.getAuth({
        hassUrl,
        saveTokens: (data) =>
            window.localStorage.setItem(STORAGE_KEY, JSON.stringify(data)),
        loadTokens: async () => {
            const data = window.localStorage.getItem(STORAGE_KEY);
            if (data) {
                return JSON.parse(data);
            }
        }
    });

const createConnection = async (
    dispatch: ThunkDispatch<unknown, unknown, AnyAction>
) => {
    const connection = await hass.createConnection({
        auth: await getAuth()
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
        const entity_id = selectSelectedPlayerId(state);
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
const PATH_PREFIX = `${hassUrl}/api/services/${DOMAIN}`;
export const playFrom = createAsyncThunk(
    'hass/playFrom',
    async (
        args: { context: IHassContext; timeStamp: number },
        { getState, dispatch }
    ) => {
        const auth = await getAuth();
        const state = getState();
        // TODO: try making this automagic by wrapping `createAsyncThunk`
        //       with a function that passes through everything, but sets
        //       the 3rd type argument to `RootState`.
        const entity_id = selectSelectedPlayerId(state as RootState);

        await Promise.all([
            axios.post(
                PATH_PREFIX + '/media_seek',
                { entity_id, seek_position: args.timeStamp },
                { headers: { Authorization: `Bearer ${auth.accessToken}` } }
            ),
            axios.post(
                PATH_PREFIX + '/media_play',
                { entity_id },
                { headers: { Authorization: `Bearer ${auth.accessToken}` } }
            )
        ]);
    }
);

interface IHassContext {
    connection?: hass.Connection;
}
export const HassContext = React.createContext<IHassContext>({});
export const useHass = () => useContext(HassContext);
