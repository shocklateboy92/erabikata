import { createAction } from '@reduxjs/toolkit';
import { HassEntities } from 'home-assistant-js-websocket';

export const hassEntityUpdate = createAction<HassEntities>('hass/entityUpdate');
export const hassSocketReady = createAction('hass/socketReady');
export const hassSocketDisconnection = createAction('hass/socketDisconnection');

export const playerSelection = createAction<string | null>(
    'hass/playerSelection'
);
