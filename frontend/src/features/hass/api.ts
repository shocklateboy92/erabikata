import { createAsyncThunk } from '@reduxjs/toolkit';
import { RootState } from 'app/rootReducer';
import * as hass from 'home-assistant-js-websocket';
import React from 'react';
import { useContext } from 'react';
import { selectSelectedPlayer } from './selectors';

const STORAGE_KEY = 'hass_state';
const getAuth = () =>
    hass.getAuth({
        hassUrl: 'https://home-assistant.apps.lasath.org',
        saveTokens: (data) =>
            window.localStorage.setItem(STORAGE_KEY, JSON.stringify(data)),
        loadTokens: async () => {
            const data = window.localStorage.getItem(STORAGE_KEY);
            if (data) {
                return JSON.parse(data);
            }
        }
    });

export const updatePlayerList = createAsyncThunk(
    'updatePlayers',
    async (context: IHassContext) => {
        if (!context.connection) {
            context.connection = await hass.createConnection({
                auth: await getAuth()
            });
        }

        const states = await hass.getStates(context.connection);
        return states
            .filter(
                (e) =>
                    e.entity_id.startsWith('media_player.plex_') &&
                    e.state !== 'unavailable'
            )
            .map((e) => ({
                name: e.attributes.friendly_name,
                id: e.entity_id
            }));
    }
);

export const play = createAsyncThunk(
    'play',
    async (context: IHassContext, { getState }) => {
        const state = getState() as RootState;
        const entity_id = selectSelectedPlayer(state);
        if (!entity_id) {
            return;
        }

        if (!context.connection) {
            context.connection = await hass.createConnection({
                auth: await getAuth()
            });
        }

        await hass.callService(context.connection, DOMAIN, 'media_pause', {
            entity_id
        });
    }
);

const DOMAIN = 'media_player';
export const playFrom = createAsyncThunk(
    'playFrom',
    async (
        args: { context: IHassContext; timeStamp: number },
        { getState }
    ) => {
        if (!args.context.connection) {
            args.context.connection = await hass.createConnection({
                auth: await getAuth()
            });
        }

        const state = getState();
        // TODO: try making this automagic by wrapping `createAsyncThunk`
        //       with a function that passes through everything, but sets
        //       the 3rd type argument to `RootState`.
        const entity_id = selectSelectedPlayer(state as RootState);

        await hass.callService(args.context.connection, DOMAIN, 'media_seek', {
            entity_id,
            seek_position: args.timeStamp
        });
        await hass.callService(args.context.connection, DOMAIN, 'media_play', {
            entity_id
        });
    }
);

interface IHassContext {
    connection?: hass.Connection;
}
export const HassContext = React.createContext<IHassContext>({});
export const useHass = () => useContext(HassContext);
